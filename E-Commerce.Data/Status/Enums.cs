using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Data.Status
{

    public enum UserRole
    {
        Admin = 1,
        Supplier = 2,
        Buyer = 3
    }

    public enum CompanyStatus
    {
        Pending = 1,
        Active = 2,
        Suspended = 3,
        Rejected = 4
    }

    public enum ProductStatus
    {
        Draft = 1,
        Active = 2,
        Inactive = 3
    }

    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6,
        Refunded = 7
    }

    public enum PaymentStatus
    {
        Unpaid = 1,
        Paid = 2,
        PartiallyPaid = 3,
        Refunded = 4
    }
    public enum PayoutStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }
    public enum RfqStatus
    {
        Pending = 1,
        Accepted = 2,
        Declined = 3,
        Expired = 4,
        Cancelled = 5,
        Quoted = 6,
        Closed = 7
    }

}
