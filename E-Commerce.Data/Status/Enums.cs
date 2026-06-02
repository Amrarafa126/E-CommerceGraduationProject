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
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Expired = 3,
        Cancelled = 4,
        Quoted = 5,
        Closed = 6
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

    public enum OrderOverallStatus
    {
        Pending = 1,
        Paid = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Completed = 6,
        Cancelled = 7,
        PartiallyCancelled = 8,
        Refunded = 9,
        PartiallyRefunded = 10
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        Video = 3,
        Voice = 4,
        File = 5,
        ProductCard = 6,
        OrderCard = 7,
        QuotationCard = 8
    }

    public enum AttachmentType
    {
        Image = 1,
        Video = 2,
        Voice = 3,
        File = 4
    }

    public enum UnitOfMeasure
    {
        Piece = 1,
        Carton = 2,
        Set = 3,
        Kg = 4,
        Meter = 5,
        Liter = 6,
        Ton = 7,
        SquareMeter = 8,
        MeterRoll = 9,
        Pair = 10,
        Dozen = 11
    }
}
