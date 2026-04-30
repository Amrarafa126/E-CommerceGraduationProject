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
        Seller = 2,
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
        Paid = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Completed = 6,
        Cancelled = 7,
        Refunded = 8
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
    public enum ShippingStatus
    {
        Pending = 1,
        ReadyForPickup = 2,
        InTransit = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Failed = 6,
        Returned = 7
    }
    public enum ShippingMethod { Standard = 1, Express = 2, Overnight = 3, Pickup = 4 }
    public enum PaymentStatus { Pending = 1, Paid = 2, Failed = 3, Refunded = 4, PartiallyRefunded = 5 }
    public enum PaymentMethod { Card = 1, Wallet = 2, CashOnDelivery = 3, BankTransfer = 4 }
}
