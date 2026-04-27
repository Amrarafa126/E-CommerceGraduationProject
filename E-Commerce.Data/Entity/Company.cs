using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;
using E_Commerce.Data.ValueObjects;


namespace E_Commerce.Data.Entity
{
    public class Company : BaseEntity
    {
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public int EmployeesCount { get; private set; }
        public int YearEstablished { get; private set; }
        public string? LogoUrl { get; set; }
        public Address Address { get; private set; } = null!;
        public ContactInfo ContactInfo { get; private set; } = null!;
        public CompanyStatus Status { get; set; } = CompanyStatus.Pending;
        public ICollection<User> Users { get; set; }
        public ICollection<Order> ReceivedOrders { get; private set; } = new List<Order>();
        public ICollection<Product> Products { get; set; }
        public ICollection<Payout> Payouts { get; private set; } = new List<Payout>();
        public Wallet? Wallet { get; private set; }


        public static Company Create(
            string name,
            string phone,
            string description,
            Address address,
            ContactInfo contactInfo,
            int yearEstablished,
            int employeesCount)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name cannot be empty.", nameof(name));

            return new Company
            {
                CompanyName = name,
                Phone = phone,
                Description = description,
                Address = address,
                ContactInfo = contactInfo,
                YearEstablished = yearEstablished,
                EmployeesCount = employeesCount,
                Status = CompanyStatus.Pending
            };
        }

        public void Update(
            string name,
            string description,
            Address address,
            ContactInfo contactInfo,
            int yearEstablished,
            int employeesCount
           )
        {
            CompanyName = name;
            Description = description;
            Address = address;
            ContactInfo = contactInfo;
            YearEstablished = yearEstablished;
            EmployeesCount = employeesCount;
            MarkAsUpdated();
        }

        public void Approve() { Status = CompanyStatus.Active; MarkAsUpdated(); }
        public void Suspend() { Status = CompanyStatus.Suspended; MarkAsUpdated(); }
        public void Reject() { Status = CompanyStatus.Rejected; MarkAsUpdated(); }
        public void SetLogo(string url) { LogoUrl = url; MarkAsUpdated(); }
        public bool IsActive => Status == CompanyStatus.Active;

    
    }
}
