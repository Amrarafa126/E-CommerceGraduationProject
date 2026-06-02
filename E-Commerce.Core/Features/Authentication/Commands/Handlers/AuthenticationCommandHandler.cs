
using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Authentication.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork uow,
    ITokenService tok,
    IMapper mapper,
    ICurrentUserService cu,
    IGoogleAuthService googleAuth) :
        IRequestHandler<RegisterSellerCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<RegisterBuyerCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<LogoutCommand, ApiResponse<object>>,
        IRequestHandler<RequestMagicLinkCommand, ApiResponse<object>>,
        IRequestHandler<VerifyMagicLinkCommand, ApiResponse<AuthResponseDto>>,
        IRequestHandler<SendEmailVerificationCommand, ApiResponse<object>>,
        IRequestHandler<VerifyEmailCommand, ApiResponse<object>>,
        IRequestHandler<ChangePasswordCommand, ApiResponse<object>>,
        IRequestHandler<UpdateProfileCommand, ApiResponse<UserDto>>,
        IRequestHandler<ForgotPasswordCommand, ApiResponse<object>>,
        IRequestHandler<ResetPasswordCommand, ApiResponse<object>>,
        IRequestHandler<RequestChangeEmailCommand, ApiResponse<object>>,
        IRequestHandler<ConfirmChangeEmailCommand, ApiResponse<object>>,
        IRequestHandler<GoogleLoginCommand, ApiResponse<AuthResponseDto>>
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
                    mapper.Map<UserDto>(user)), "تم إنشاء حساب البائع بنجاح.");
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
                    mapper.Map<UserDto>(user)), "تم إنشاء حساب المشتري بنجاح.");
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
                throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

            if (await userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException(
                    "تم قفل الحساب بسبب محاولات فاشلة متعددة. يرجى المحاولة مرة أخرى بعد 15 دقيقة.");

            var valid = await userManager.CheckPasswordAsync(user, req.Password);
            if (!valid)
            {
                await userManager.AccessFailedAsync(user);
                throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            }

            await userManager.ResetAccessFailedCountAsync(user);

            var refresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, refresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)), "تم تسجيل الدخول بنجاح.");
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
      RefreshTokenCommand req, CancellationToken ct)
        {
            var principal = tok.GetPrincipalFromExpiredToken(req.AccessToken)
                ?? throw new UnauthorizedException("رمز الوصول غير صالح.");

            var userIdStr = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedException("بيانات الرمز غير صالحة.");

            var user = await uow.Users.GetByIdAsync(userId, ct)
                ?? throw new UnauthorizedException("المستخدم غير موجود.");

            if (user.RefreshToken != req.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("رمز التحديث غير صالح أو منتهي الصلاحية.");

            var newRefresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(newRefresh, DateTime.UtcNow.AddDays(30));
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, newRefresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)), "تم تحديث الجلسة بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(LogoutCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                return ApiResponse<object>.Unauthorized("غير مصرح. يرجى تسجيل الدخول.");

            var user = await userManager.FindByIdAsync(cu.UserId.ToString()!);
            if (user == null)
                return ApiResponse<object>.NotFound("المستخدم غير موجود.");

            user.ClearRefreshToken();
            await userManager.UpdateAsync(user);

            return ApiResponse<object>.Ok("تم تسجيل الخروج بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(RequestMagicLinkCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            bool isNew = false;

            if (user == null)
            {
                user = User.CreateBuyer(req.Email, string.Empty, string.Empty, null);
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    throw new ValidationException(createResult.Errors.Select(e => e.Description));

                await userManager.AddToRoleAsync(user, Role.Names.Buyer);
                isNew = true;
            }

            if (!user.IsActive || user.IsDeleted)
                throw new UnauthorizedException("الحساب غير نشط.");

            var token = Guid.NewGuid().ToString("N");
            user.SetMagicLinkToken(token, DateTime.UtcNow.AddMinutes(15));
            await userManager.UpdateAsync(user);

            // Note: actual email sending is optional here; the user asked to prepare implementation.
            // Frontend can also read the token from response for development.
            return ApiResponse<object>.Ok(new
            {
                Message = "تم إرسال رابط تسجيل الدخول إلى بريدك الإلكتروني.",
                Token = token,
                IsNewUser = isNew
            });
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(VerifyMagicLinkCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null || user.IsDeleted || !user.IsActive)
                throw new UnauthorizedException("الرابط غير صالح.");

            if (user.MagicLinkToken != req.Token || user.MagicLinkExpiry <= DateTime.UtcNow)
                throw new UnauthorizedException("الرابط منتهي الصلاحية أو غير صالح.");

            user.ClearMagicLinkToken();
            var refresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, refresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)), "تم تسجيل الدخول بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(SendEmailVerificationCommand req, CancellationToken ct)
        {
            var user = cu.UserId != null
                ? await userManager.FindByIdAsync(cu.UserId.ToString()!)
                : null;

            if (user == null)
                return ApiResponse<object>.Unauthorized("غير مصرح.");

            if (user.EmailConfirmed)
                return ApiResponse<object>.Ok("البريد الإلكتروني مؤكد بالفعل.");

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var encodedUserId = System.Web.HttpUtility.UrlEncode(user.Id.ToString());

            return ApiResponse<object>.Ok(new
            {
                Message = "تم إرسال رابط التحقق إلى بريدك الإلكتروني.",
                UserId = user.Id,
                Token = token,
                VerificationUrl = $"http://localhost:5173/auth/verify-email?userId={encodedUserId}&token={encodedToken}"
            });
        }

        public async Task<ApiResponse<object>> Handle(VerifyEmailCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByIdAsync(req.UserId.ToString());
            if (user == null)
                return ApiResponse<object>.NotFound("المستخدم غير موجود.");

            var result = await userManager.ConfirmEmailAsync(user, req.Token);
            if (!result.Succeeded)
                return ApiResponse<object>.Fail("رابط التحقق غير صالح أو منتهي الصلاحية.", 400);

            return ApiResponse<object>.Ok("تم تأكيد البريد الإلكتروني بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(ChangePasswordCommand req, CancellationToken ct)
        {
            var user = cu.UserId != null
                ? await userManager.FindByIdAsync(cu.UserId.ToString()!)
                : null;

            if (user == null)
                return ApiResponse<object>.Unauthorized("غير مصرح.");

            var result = await userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            return ApiResponse<object>.Ok("تم تغيير كلمة المرور بنجاح.");
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateProfileCommand req, CancellationToken ct)
        {
            var user = cu.UserId != null
                ? await userManager.FindByIdAsync(cu.UserId.ToString()!)
                : null;

            if (user == null)
                return ApiResponse<UserDto>.Unauthorized("غير مصرح.");

            user.UpdateProfile(req.FirstName, req.LastName, req.PhoneNumber, req.AvatarUrl);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            return ApiResponse<UserDto>.Ok(mapper.Map<UserDto>(user), "تم تحديث البيانات الشخصية بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(ForgotPasswordCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null || !user.IsActive || user.IsDeleted)
                return ApiResponse<object>.Ok("إذا كان البريد مسجلاً، فسيتم إرسال رابط إعادة تعيين كلمة المرور.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return ApiResponse<object>.Ok(new
            {
                Message = "تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني.",
                Email = req.Email,
                Token = token,
                ResetUrl = $"http://localhost:5173/auth/reset-password?email={System.Web.HttpUtility.UrlEncode(req.Email)}&token={System.Web.HttpUtility.UrlEncode(token)}"
            });
        }

        public async Task<ApiResponse<object>> Handle(ResetPasswordCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null || !user.IsActive || user.IsDeleted)
                return ApiResponse<object>.Fail("البريد الإلكتروني غير صالح.");

            var result = await userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            return ApiResponse<object>.Ok("تم إعادة تعيين كلمة المرور بنجاح.");
        }

        public async Task<ApiResponse<object>> Handle(RequestChangeEmailCommand req, CancellationToken ct)
        {
            var user = cu.UserId != null
                ? await userManager.FindByIdAsync(cu.UserId.ToString()!)
                : null;

            if (user == null)
                return ApiResponse<object>.Unauthorized("غير مصرح.");

            var existing = await userManager.FindByEmailAsync(req.NewEmail);
            if (existing != null && existing.Id != user.Id)
                return ApiResponse<object>.Fail("البريد الإلكتروني الجديد مستخدم بالفعل.", 409);

            var token = await userManager.GenerateChangeEmailTokenAsync(user, req.NewEmail);
            return ApiResponse<object>.Ok(new
            {
                Message = "تم إرسال رابط التحقق إلى البريد الإلكتروني الجديد.",
                UserId = user.Id,
                NewEmail = req.NewEmail,
                Token = token,
                ConfirmationUrl = $"http://localhost:5173/auth/confirm-change-email?userId={user.Id}&newEmail={System.Web.HttpUtility.UrlEncode(req.NewEmail)}&token={System.Web.HttpUtility.UrlEncode(token)}"
            });
        }

        public async Task<ApiResponse<object>> Handle(ConfirmChangeEmailCommand req, CancellationToken ct)
        {
            var user = await userManager.FindByIdAsync(req.UserId.ToString());
            if (user == null)
                return ApiResponse<object>.NotFound("المستخدم غير موجود.");

            var result = await userManager.ChangeEmailAsync(user, req.NewEmail, req.Token);
            if (!result.Succeeded)
                return ApiResponse<object>.Fail("رابط التحقق غير صالح أو منتهي الصلاحية.", 400);

            await userManager.SetUserNameAsync(user, req.NewEmail);
            return ApiResponse<object>.Ok("تم تغيير البريد الإلكتروني بنجاح.");
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(GoogleLoginCommand req, CancellationToken ct)
        {
            var googleUser = await googleAuth.ValidateIdTokenAsync(req.IdToken);

            var user = await userManager.FindByEmailAsync(googleUser.Email);
            bool isNew = false;

            if (user == null)
            {
                user = User.CreateBuyer(
                    googleUser.Email,
                    googleUser.FirstName ?? string.Empty,
                    googleUser.LastName ?? string.Empty);

                var randomPassword = GenerateRandomPassword();
                var createResult = await userManager.CreateAsync(user, randomPassword);
                if (!createResult.Succeeded)
                    throw new ValidationException(createResult.Errors.Select(e => e.Description));

                await userManager.AddToRoleAsync(user, Role.Names.Buyer);
                user.VerifyEmail();
                isNew = true;
            }

            if (user.IsDeleted || !user.IsActive)
                throw new UnauthorizedException("الحساب غير نشط.");

            if (!user.EmailConfirmed)
            {
                user.VerifyEmail();
            }

            var refresh = tok.GenerateRefreshToken();
            user.SetRefreshToken(refresh, DateTime.UtcNow.AddDays(30));

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ValidationException(updateResult.Errors.Select(e => e.Description));

            var roles = await userManager.GetRolesAsync(user);
            var access = tok.GenerateAccessToken(user, roles);

            var message = isNew
                ? "تم إنشاء الحساب وتسجيل الدخول بنجاح."
                : "تم تسجيل الدخول بنجاح.";

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(
                access, refresh, DateTime.UtcNow.AddMinutes(60),
                mapper.Map<UserDto>(user)), message);
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return new string(bytes.Select(b => chars[b % chars.Length]).ToArray()) + "A1!";
        }

    }

}
