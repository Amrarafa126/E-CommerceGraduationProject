using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class User
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? Role { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
