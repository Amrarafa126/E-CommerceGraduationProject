using E_Commerce.Core.Features.Orders.Commands.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Orders.Commands.Vaildations
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.SellerCompanyId).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty().Length(3);
            RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
