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

        // ── Refresh token (stored here for single-table lookup) ────────
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        // ── Company links ──────────────────────────────────────────────
        /// <summary>Set only for Seller role. 1-to-1 with Company.</summary>
        public Guid? OwnedCompanyId { get; private set; }
        public Company? OwnedCompany { get; private set; }

  
        // ── Audit / Soft-delete (Identity doesn't have these) ─────────
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        // ── Navigation ─────────────────────────────────────────────────
        public ICollection<Order> Orders { get; private set; } = new List<Order>();
        public ICollection<ProductReview> Reviews { get; private set; } = new List<ProductReview>();

        // ── Constructors ───────────────────────────────────────────────
        // Identity requires a parameterless constructor
        public User() { }

        // ── Factory methods ────────────────────────────────────────────
        public static User CreateSupplier(string email, string firstName, string lastName, string? phone = null)
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
                Role = UserRole.Supplier,
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

        // ── Domain behaviour ───────────────────────────────────────────
        public void AssignOwnedCompany(Guid companyId)
        {
            if (Role != UserRole.Supplier)
                throw new InvalidOperationException("Only Sellers can own a company.");
            OwnedCompanyId = companyId;
            MarkAsUpdated();
        }

        //public void JoinCompanyAsEmployee(Guid companyId)
        //{
        //    EmployerCompanyId = companyId;
        //    MarkAsUpdated();
        //}

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

        public void UpdateProfile(string firstName, string lastName, string? phone)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phone;
            MarkAsUpdated();
        }

        public void SoftDelete() { IsDeleted = true; MarkAsUpdated(); }

        private void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

        // ── Computed ───────────────────────────────────────────────────
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsSupplier => Role == UserRole.Supplier;
        public bool IsBuyer => Role == UserRole.Buyer;
        public bool IsAdmin => Role == UserRole.Admin;


    }
}
