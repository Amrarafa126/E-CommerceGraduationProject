using System.ComponentModel.DataAnnotations.Schema;
using E_Commerce.Data.Entity;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Data.Identity
{
    public class User : IdentityUser
    {
        public User() {
           Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }
      
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
