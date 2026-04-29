using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Models
{
    public record InitiatePaymentCommand(
       Guid OrderId,
       string PaymentMethod,
       string? CardToken,
       string Currency = "EGP")
       : IRequest<ApiResponse<PaymentDto>>;
}
