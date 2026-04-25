using E_Commerce.Data.Entity;
using E_Commerce.Data.Result;
using E_Commerce.Data.Status;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Identity
{
    public class User : IdentityUser 
    {

        public string? FirstName { get;  set; } 
        public string? LastName { get;  set; } 
        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public UserRole role { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<ProductReview>? Reviews { get; set; }
        public string? RefreshToken { get;  set; }
        public DateTime? RefreshTokenExpiryTime { get;  set; }
      //  public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;
        }


        public static User CreateSupplier(
       string email, string passwordHash,
       string firstName, string lastName, string? phone = null)
        {
            return new User
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                role = UserRole.Supplier
            };
        }

        public static User CreateBuyer(
            string email, string passwordHash,
            string firstName, string lastName, string? phone = null)
        {
            return new User
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                role = UserRole.Buyer
            };
        }

        public void AssignCompany(Guid companyId) => CompanyId = companyId;
       // public void VerifyEmail() => IsEmailVerified = true;
        public string FullName => $"{FirstName} {LastName}";

      

    }
}
