using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ProductOptions.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.ProductOptions.Commands.Handlers
{
   
        public class AddProductOptionHandler(IUnitOfWork uow, IMapper mapper)
     : IRequestHandler<AddProductOptionCommand, ApiResponse<ProductOptionDto>>
        {
            public async Task<ApiResponse<ProductOptionDto>> Handle(AddProductOptionCommand req, CancellationToken ct)
            {
                var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                    ?? throw new NotFoundException(nameof(Product), req.ProductId);

                var option = ProductOption.Create(product.Id, req.Name);

                if (req.Values != null)
                {
                    foreach (var val in req.Values)
                        option.Values.Add(ProductOptionValue.Create(option.Id, val)); // خلي بالك انك لازم تضيف ال option قبل ما تضيف ال values عشان تاخد ال optionId اللي هو auto increment
            }

                product.ProductOptions.Add(option);
                uow.Products.Update(product);
                await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductOptionDto>.Created(mapper.Map<ProductOptionDto>(option));
        }
    }
 }

