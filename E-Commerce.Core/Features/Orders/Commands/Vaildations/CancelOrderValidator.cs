using E_Commerce.Core.Features.Orders.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Orders.Commands.Vaildations
{
    public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}
