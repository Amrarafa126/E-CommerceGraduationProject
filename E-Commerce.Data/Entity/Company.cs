using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class Company
    {
        public Company()
        {
            Users = new HashSet<User>();
            Products = new HashSet<Product>();
        }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverUrl { get; set; }
        public bool IsActive { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Product> Products { get; set; }
    }
    public enum VerificationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
