
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Org.BouncyCastle.Ocsp;

namespace E_Commerce.Core.Features.Products.Commands.Handlers
{
    public class ProductHandlerCommand(
    IUnitOfWork uow,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<AddProductModelComands, ApiResponse<ProductDto>>
    {
       
        public async Task<ApiResponse<ProductDto>> Handle(AddProductModelComands req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var user = await uow.Users.GetWithCompanyAsync(currentUser.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(ApplicationUser), currentUser.UserId.Value);

            if (user.CompanyId == null)
                throw new BusinessException("You must belong to a company to add products.");

            var categoryExists = await uow.Companies.ExistsAsync(
                _ => true, ct); // placeholder — use category repo when available

            var product = Product.Create(
                req.Name,
                req.Description,
                user.CompanyId, 
                req.CategoryId,
                req.MinimumOrderQuantity,
                req.BasePrice,
                req.Currency);

            await uow.Products.AddAsync(product, ct);
            await uow.SaveChangesAsync(ct);

            var full = await uow.Products.GetWithFullDetailsAsync(product.Id, ct);
            return ApiResponse<ProductDto>.Created(mapper.Map<ProductDto>(full!));

        }
    }
}
    

