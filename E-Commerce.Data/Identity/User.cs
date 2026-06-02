using E_Commerce.Data.Entity;
using E_Commerce.Data.Result;
using E_Commerce.Data.Status;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Data.Identity
{
    public class User : IdentityUser<Guid> 
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? AvatarUrl { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public string? MagicLinkToken { get; private set; }
        public DateTime? MagicLinkExpiry { get; private set; }
        public Guid? OwnedCompanyId { get; private set; }
        public Company? OwnedCompany { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

      
        public ICollection<Order> Orders { get; private set; } = new List<Order>();
        public ICollection<ProductReview> Reviews { get; private set; } = new List<ProductReview>();

     
        public User() { }

        public static User CreateSeller(string email, string firstName, string lastName, string? phone = null)
            => new()
            {
                Id = Guid.NewGuid(),
                Email = email.ToLower().Trim(),
                UserName = email.ToLower().Trim(),   // Identity requires UserName
                NormalizedEmail = email.ToUpper().Trim(),
                NormalizedUserName = email.ToUpper().Trim(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                Role = UserRole.Seller,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

        public static User CreateBuyer(string email, string firstName, string lastName, string? phone = null)
            => new()
            {
                Id = Guid.NewGuid(),
                Email = email.ToLower().Trim(),
                UserName = email.ToLower().Trim(),
                NormalizedEmail = email.ToUpper().Trim(),
                NormalizedUserName = email.ToUpper().Trim(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                Role = UserRole.Buyer,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

        public static User CreateAdmin(string email, string firstName, string lastName)
            => new()
            {
                Id = Guid.NewGuid(),
                Email = email.ToLower().Trim(),
                UserName = email.ToLower().Trim(),
                NormalizedEmail = email.ToUpper().Trim(),
                NormalizedUserName = email.ToUpper().Trim(),
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.Admin,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

        public void AssignOwnedCompany(Guid companyId)
        {
            if (Role != UserRole.Seller)
                throw new InvalidOperationException("Only Sellers can own a company.");
            OwnedCompanyId = companyId;
            MarkAsUpdated();
        }

  

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiry;
            MarkAsUpdated();
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            MarkAsUpdated();
        }

        public void VerifyEmail()
        {
            EmailConfirmed = true;
            MarkAsUpdated();
        }

        public void Deactivate() { IsActive = false; MarkAsUpdated(); }
        public void Activate() { IsActive = true; MarkAsUpdated(); }

        public void SetAvatar(string url) { AvatarUrl = url; MarkAsUpdated(); }

        public void UpdateProfile(string firstName, string lastName, string? phone, string? avatarUrl = null)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            if (avatarUrl != null)
                AvatarUrl = avatarUrl;
            MarkAsUpdated();
        }

        public void SetMagicLinkToken(string token, DateTime expiry)
        {
            MagicLinkToken = token;
            MagicLinkExpiry = expiry;
            MarkAsUpdated();
        }

        public void ClearMagicLinkToken()
        {
            MagicLinkToken = null;
            MagicLinkExpiry = null;
            MarkAsUpdated();
        }

        public void SoftDelete() { IsDeleted = true; MarkAsUpdated(); }

        private void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsSeller => Role == UserRole.Seller;
        public bool IsBuyer => Role == UserRole.Buyer;
        public bool IsAdmin => Role == UserRole.Admin;


    }
}
