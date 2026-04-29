using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Commands.Models
{
    public record RefundPaymentCommand(Guid PaymentId, decimal Amount, string Reason)
    : IRequest<ApiResponse<PaymentDto>>;
}
