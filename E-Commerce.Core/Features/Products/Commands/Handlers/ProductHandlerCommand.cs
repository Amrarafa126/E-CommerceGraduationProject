
using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Products.Commands.Handlers
{
    public class ProductHandlerCommand(
    IUnitOfWork uow,
    ICurrentUserService cu,
    IMapper mapper) :
        IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>,
        IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>,
        IRequestHandler<DeleteProductCommand, ApiResponse<object>>,
        IRequestHandler<PublishProductCommand, ApiResponse<ProductDto>>
    {
       
        public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            // Use OwnedCompanyId directly from JWT — no extra DB call
            if (cu.OwnedCompanyId == null)
                throw new ForbiddenException("Only Sellers with an active company can create products.");

            var company = await uow.Companies.GetByIdAsync(cu.OwnedCompanyId.Value, ct)
                ?? throw new NotFoundException(nameof(Company), cu.OwnedCompanyId.Value);

            if (!company.IsActive)
                throw new BusinessException("Your company must be approved before adding products.");

            var product = Product.Create(req.Name, req.Description,
                cu.OwnedCompanyId.Value, req.CategoryId,
                req.MinimumOrderQuantity, req.BasePrice, req.Currency,
                req.StockQuantity);

            // Apply B2B fields
            product.Brand = req.Brand;
            product.ModelNumber = req.ModelNumber;
            product.OriginCountry = req.OriginCountry;
            product.UnitOfMeasure = (UnitOfMeasure)(req.UnitOfMeasure ?? (int)UnitOfMeasure.Piece);
            product.LeadTimeDays = req.LeadTimeDays ?? 0;
            product.LeadTimeDaysSample = req.LeadTimeDaysSample;
            product.SupplyAbility = req.SupplyAbility;
            product.TradeTerms = req.TradeTerms;
            product.PortOfLoading = req.PortOfLoading;
            product.PaymentTerms = req.PaymentTerms;
            product.PackagingDetails = req.PackagingDetails;
            product.SampleAvailable = req.SampleAvailable ?? false;
            product.SamplePrice = req.SamplePrice;
            product.SampleMoq = req.SampleMoq;
            product.MetaTitle = req.MetaTitle;
            product.MetaDescription = req.MetaDescription;
            product.Slug = req.Slug;

            await uow.Products.AddAsync(product, ct);
            await uow.SaveChangesAsync(ct);

            var full = await uow.Products.GetWithFullDetailsAsync(product.Id, ct);
            return ApiResponse<ProductDto>.Created(ProductMapper.Map(full!));
        }
        public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("يمكنك تعديل منتجاتك فقط.");

            product.Update(req.Name, req.Description, req.CategoryId,
                req.MinimumOrderQuantity, req.BasePrice, req.StockQuantity);

            // Apply B2B fields
            product.Brand = req.Brand;
            product.ModelNumber = req.ModelNumber;
            product.OriginCountry = req.OriginCountry;
            if (req.UnitOfMeasure.HasValue)
                product.UnitOfMeasure = (UnitOfMeasure)req.UnitOfMeasure.Value;
            if (req.LeadTimeDays.HasValue)
                product.LeadTimeDays = req.LeadTimeDays.Value;
            product.LeadTimeDaysSample = req.LeadTimeDaysSample;
            product.SupplyAbility = req.SupplyAbility;
            product.TradeTerms = req.TradeTerms;
            product.PortOfLoading = req.PortOfLoading;
            product.PaymentTerms = req.PaymentTerms;
            product.PackagingDetails = req.PackagingDetails;
            if (req.SampleAvailable.HasValue)
                product.SampleAvailable = req.SampleAvailable.Value;
            product.SamplePrice = req.SamplePrice;
            product.SampleMoq = req.SampleMoq;
            product.MetaTitle = req.MetaTitle;
            product.MetaDescription = req.MetaDescription;
            product.Slug = req.Slug;

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductDto>.Ok(ProductMapper.Map(product));
        }
        public async Task<ApiResponse<object>> Handle(DeleteProductCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var product = await uow.Products.GetByIdAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only delete your own products.");

            product.SoftDelete();
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<object>.Ok("Product deleted.");
        }
        public async Task<ApiResponse<ProductDto>> Handle(PublishProductCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var product = await uow.Products.GetWithFullDetailsAsync(req.ProductId, ct)
                ?? throw new NotFoundException(nameof(Product), req.ProductId);

            if (cu.OwnedCompanyId != product.CompanyId && cu.Role != "Admin")
                throw new ForbiddenException("You can only publish your own products.");

            product.Publish();
            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<ProductDto>.Ok(ProductMapper.Map(product));
        }
    }
}
