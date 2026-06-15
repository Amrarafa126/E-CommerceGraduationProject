
using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Products.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System.Text;

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
            product.Slug = await EnsureUniqueSlugAsync(req.Slug, req.Name, null, ct);

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
            product.Slug = await EnsureUniqueSlugAsync(req.Slug, req.Name, product.Id, ct);

            uow.Products.Update(product);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<ProductDto>.Ok(ProductMapper.Map(product));
        }

        private async Task<string?> EnsureUniqueSlugAsync(string? slug, string name, Guid? excludeProductId, CancellationToken ct)
        {
            var candidate = string.IsNullOrWhiteSpace(slug) ? GenerateSlug(name) : slug.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(candidate))
                return null;

            const int maxLength = 300;
            if (candidate.Length > maxLength)
                candidate = candidate[..maxLength];

            var baseSlug = candidate;
            int suffix = 2;

            while (excludeProductId.HasValue
                ? await uow.Products.ExistsAsync(p => p.Slug == candidate && p.Id != excludeProductId.Value, ct)
                : await uow.Products.ExistsAsync(p => p.Slug == candidate, ct))
            {
                var suffixText = $"-{suffix}";
                candidate = baseSlug.Length + suffixText.Length > maxLength
                    ? baseSlug[..(maxLength - suffixText.Length)] + suffixText
                    : baseSlug + suffixText;
                suffix++;
            }

            return candidate;
        }

        private static string GenerateSlug(string name)
        {
            var allowed = name
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')
                .ToArray();

            var sb = new StringBuilder();
            bool lastWasHyphen = false;

            foreach (var c in allowed)
            {
                if (char.IsWhiteSpace(c) || c == '-')
                {
                    if (!lastWasHyphen)
                    {
                        sb.Append('-');
                        lastWasHyphen = true;
                    }
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                    lastWasHyphen = false;
                }
            }

            return sb.ToString().Trim('-');
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
