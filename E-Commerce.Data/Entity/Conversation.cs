using E_Commerce.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Entity
{
    public class Conversation : BaseEntity
    {
        public Guid BuyerId { get; private set; }
        public User Buyer { get; private set; } = null!;
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        public bool IsBuyerArchived { get; private set; }
        public bool IsCompanyArchived { get; private set; }
        public DateTime? LastMessageAt { get; private set; }
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        private Conversation() { }
        public static Conversation Create(Guid buyerId, Guid companyId) => new()
        {
            BuyerId = buyerId,
            CompanyId = companyId,
            LastMessageAt = DateTime.UtcNow
        };

        public void TouchLastMessage()
        {
            LastMessageAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
        public void ArchiveForBuyer() { IsBuyerArchived = true; MarkAsUpdated(); }
        public void ArchiveForCompany() { IsCompanyArchived = true; MarkAsUpdated(); }
    }
}
