using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }

        public bool IsVerified { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
