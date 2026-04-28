
using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using E_Commerce.Core.Features.Companies;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Azure.Core.HttpHeader;

namespace E_Commerce.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork uow,
    ITokenService tok,
    IMapper mapper) :
        IRequestHandler<RegisterSellerCommand, ApiResponse<AuthResponseDto>>
    {
        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterSellerCommand req, CancellationToken ct)
        {
            // Build domain user — no password here, Identity sets it via CreateAsync
            var user = User.CreateSupplier(
                req.Email, req.FirstName, req.LastName, req.PhoneNumber);

            await uow.BeginTransactionAsync(ct);
            try
            {
                // ── 1. Identity: create user + hash password ──────────
                var result = await userManager.CreateAsync(user, req.Password);
                if (!result.Succeeded)
                    throw new ValidationException(
                         result.Errors.Select(e => e.Description));

                // ── 2. Identity: assign "Seller" role ─────────────────
                await userManager.AddToRoleAsync(user, Role.Names.Supplier);

                // ── 3. Domain: Company + Wallet ───────────────────────
                var address = new Address(req.Street, req.City, req.State, req.Country, req.PostalCode);
                var contact = new ContactInfo(req.ContactEmail, req.ContactPhone);
                var company = Company.Create(user.Id, req.CompanyName, req.CompanyDescription,
                    address, contact, req.YearEstablished, req.EmployeesCount);

                await uow.Companies.AddAsync(company, ct);
                await uow.SaveChangesAsync(ct);

                var wallet = Wallet.Create(company.Id);
                await uow.Wallets.AddAsync(wallet, ct);

                // ── 4. Link user → company ─────────────────────────────
                user.AssignOwnedCompany(company.Id);

                var refresh = tok.GenerateRefreshToken();
                user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));

                // Persist domain-property changes through UserManager
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new ValidationException(
                    result.Errors.Select(e => e.Description));

                await uow.SaveChangesAsync(ct);
                await uow.CommitTransactionAsync(ct);

                var roles = await userManager.GetRolesAsync(user);
                var access = tok.GenerateAccessToken(user, roles);

                return ApiResponse<AuthResponseDto>.Created(new AuthResponseDto(
                    access, refresh, DateTime.UtcNow.AddMinutes(60),
                    mapper.Map<UserDto>(user)));
            }
            catch { await uow.RollbackTransactionAsync(ct); throw; }
        }
    }

}

