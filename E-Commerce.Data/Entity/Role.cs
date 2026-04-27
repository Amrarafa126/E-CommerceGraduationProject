using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Role : IdentityRole
    {
        public int Id { get; set; }
        public Role()
        {
            
        }
    }
}
