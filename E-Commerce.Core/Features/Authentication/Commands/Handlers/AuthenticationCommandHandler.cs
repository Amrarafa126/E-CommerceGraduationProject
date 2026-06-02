
using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork uow,
    ITokenService tok,
    IMapper mapper,
    ICurrentUserService cu) :
        IRequestHandler<RegisterSellerCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<RegisterBuyerCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<LogoutCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterSellerCommand req, CancellationToken ct)
        {
            var user = User.CreateSeller(
                req.Email, req.FirstName, req.LastName, req.PhoneNumber);
            bool userCreated = false;

            await uow.BeginTransactionAsync(ct);
            try
            {
                var result = await userManager.CreateAsync(user, req.Password);
                if (!result.Succeeded)
                    throw new ValidationException(
                         result.Errors.Select(e => e.Description));
                userCreated = true;

                await userManager.AddToRoleAsync(user, Role.Names.Seller);

                var address = new Address(req.Street, req.City, req.State, req.Country, req.PostalCode);
                var contact = new ContactInfo(req.ContactEmail, req.ContactPhone);
                var company = Company.Create(user.Id, req.CompanyName, req.CompanyDescription,
                    address, contact, req.YearEstablished, req.EmployeesCount);

                await uow.Companies.AddAsync(company, ct);

                var wallet = Wallet.Create(company.Id);
                await uow.Wallets.AddAsync(wallet, ct);

                user.AssignOwnedCompany(company.Id);

                var refresh = tok.GenerateRefreshToken();
                user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new ValidationException(
                    updateResult.Errors.Select(e => e.Description));

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var roles = await userManager.GetRolesAsync(user);
                var access = tok.GenerateAccessToken(user, roles);

                return ApiResponse<AuthResponseDto>.Created(new AuthResponseDto(
                    access, refresh, DateTime.UtcNow.AddMinutes(60),
                    mapper.Map<UserDto>(user)));
            }
            catch
            {
                await uow.RollbackTransactionAsync(ct);
                if (userCreated)
                {
                    try { await userManager.DeleteAsync(user); } catch { /* best effort cleanup */ }
                }
                throw;
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
       RegisterBuyerCommand req, CancellationToken ct)
        {
            var user = User.CreateBuyer(
                req.Email, req.FirstName, req.LastName, req.PhoneNumber);
            bool userCreated = false;

            await uow.BeginTransactionAsync(ct);
            try
            {
                var result = await userManager.CreateAsync(user, req.Password);
                if (!result.Succeeded)
                    throw new ValidationException(
                        result.Errors.Select(e => e.Description));
                userCreated = true;

                await userManager.AddToRoleAsync(user, Role.Names.Buyer);

                var refresh = tok.GenerateRefreshToken();
                user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new ValidationException(
                        updateResult.Errors.Select(e => e.Description));

                await uow.CommitTransactionAsync(ct);

                var roles = await userManager.GetRolesAsync(user);
                var access = tok.GenerateAccessToken(user, roles);

                return ApiResponse<AuthResponseDto>.Created(new AuthResponseDto(
                    access, refresh, DateTime.UtcNow.AddMinutes(60),
                    mapper.Map<UserDto>(user)));
            }
            catch
            {
                await uow.RollbackTransactionAsync(ct);
                if (userCreated)
                {
                    try { await userManager.DeleteAsync(user); } catch { /* best effort cleanup */ }
                }
                throw;
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
       LoginCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);

            if (user is null || user.IsDeleted || !user.IsActive)
                throw new UnauthorizedException("Invalid email or password.");

            if (await userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException(
                    "Account is locked out due to multiple failed login attempts. " +
                    "Try again in 15 minutes.");

            var valid = await userManager.CheckPasswordAsync(user, req.Password);
            if (!valid)
            {
                await userManager.AccessFailedAsync(user);
                throw new UnauthorizedException("Invalid email or password.");
            }

            await userManager.ResetAccessFailedCountAsync(user);

            var refresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, refresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)));
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
      RefreshTokenCommand req, CancellationToken ct)
        {
            var principal = tok.GetPrincipalFromExpiredToken(req.AccessToken)
                ?? throw new UnauthorizedException("Invalid access token.");

            var userIdStr = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedException("Invalid token claims.");

            var user = await uow.Users.GetByIdAsync(userId, ct)
                ?? throw new UnauthorizedException("User not found.");

            if (user.RefreshToken != req.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token is invalid or expired.");

            var newRefresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(newRefresh, DateTime.UtcNow.AddDays(30));
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, newRefresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)));
        }

        public async Task<ApiResponse<object>> Handle(LogoutCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                return ApiResponse<object>.Fail("Unauthorized.", 401);

            var user = await userManager.FindByIdAsync(cu.UserId.ToString()!);
            if (user == null)
                return ApiResponse<object>.Fail("User not found.", 404);

            user.ClearRefreshToken();
            await userManager.UpdateAsync(user);

            return ApiResponse<object>.Ok("Logged out successfully.");
        }
    }

}

