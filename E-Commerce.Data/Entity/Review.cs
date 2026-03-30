using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Identity;

namespace E_Commerce.Data.Entity
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
