using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Role : IdentityRole<Guid>
    {
        public string? Description { get; set; }

        public Role() : base() { }

        public Role(string roleName, string? description = null)
            : base(roleName)
        {
            Id = Guid.NewGuid();
            Description = description;
            NormalizedName = roleName.ToUpperInvariant();
        }

        // ── Role name constants (matches UserRole enum) ────────────────
        public static class Names
        {
            public const string Admin = "Admin";
            public const string Seller = "Seller";
            public const string Buyer = "Buyer";

            public static readonly string[] All = [Admin, Seller, Buyer];
        }
    }
}
