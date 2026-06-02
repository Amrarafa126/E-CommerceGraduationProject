using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Infrustructure.Identity
{
    public class ArabicIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "حدث خطأ غير متوقع." };

        public override IdentityError ConcurrencyFailure()
            => new() { Code = nameof(ConcurrencyFailure), Description = "تم تعديل البيانات من قبل مستخدم آخر. يرجى المحاولة مرة أخرى." };

        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "كلمة المرور الحالية غير صحيحة." };

        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "الرمز غير صالح أو منتهي الصلاحية." };

        public override IdentityError LoginAlreadyAssociated()
            => new() { Code = nameof(LoginAlreadyAssociated), Description = "هذا الحساب مرتبط بطريقة تسجيل دخول أخرى." };

        public override IdentityError InvalidUserName(string? userName)
            => new() { Code = nameof(InvalidUserName), Description = $"اسم المستخدم '{userName}' غير صالح. يجب أن يحتوي على أحرف وأرقام فقط." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = $"البريد الإلكتروني '{email}' غير صالح." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"اسم المستخدم '{userName}' مستخدم بالفعل." };

        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"البريد الإلكتروني '{email}' مستخدم بالفعل." };

        public override IdentityError InvalidRoleName(string? role)
            => new() { Code = nameof(InvalidRoleName), Description = $"اسم الدور '{role}' غير صالح." };

        public override IdentityError DuplicateRoleName(string role)
            => new() { Code = nameof(DuplicateRoleName), Description = $"الدور '{role}' موجود بالفعل." };

        public override IdentityError UserAlreadyInRole(string role)
            => new() { Code = nameof(UserAlreadyInRole), Description = "المستخدم لديه هذا الدور بالفعل." };

        public override IdentityError UserNotInRole(string role)
            => new() { Code = nameof(UserNotInRole), Description = "المستخدم ليس لديه هذا الدور." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"يجب أن تكون كلمة المرور {length} أحرف على الأقل." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "يجب أن تحتوي كلمة المرور على رمز خاص." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"يجب أن تحتوي كلمة المرور على {uniqueChars} أحرف مميزة على الأقل." };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new() { Code = nameof(RecoveryCodeRedemptionFailed), Description = "فشل استخدام رمز الاسترداد." };

        public override IdentityError UserAlreadyHasPassword()
            => new() { Code = nameof(UserAlreadyHasPassword), Description = "المستخدم لديه كلمة مرور بالفعل." };
    }
}
