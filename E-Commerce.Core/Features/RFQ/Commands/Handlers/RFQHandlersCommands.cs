using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.RFQ.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.RFQ.Commands.Handlers
{
    public class RFQHandlersCommands(IUnitOfWork uow, ICurrentUserService cu, IMapper mapper)
    : IRequestHandler<CreateRfqCommand, ApiResponse<RfqRequestDto>>,
      IRequestHandler<CancelRfqCommand, ApiResponse<RfqRequestDto>>,
      IRequestHandler<SubmitQuoteCommand, ApiResponse<RfqQuoteDto>>,
      IRequestHandler<AcceptQuoteCommand, ApiResponse<RfqRequestDto>>,
      IRequestHandler<DeclineQuoteCommand, ApiResponse<RfqRequestDto>>,
      IRequestHandler<DeclineRfqCommand, ApiResponse<RfqRequestDto>>
    {
        public async Task<ApiResponse<RfqRequestDto>> Handle(CreateRfqCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            if (req.SellerCompanyId.HasValue)
            {
                var company = await uow.Companies.GetByIdAsync(req.SellerCompanyId.Value, ct)
                    ?? throw new NotFoundException(nameof(Company), req.SellerCompanyId.Value);

                if (company.Status != CompanyStatus.Active)
                    throw new BusinessException("لا يمكن إرسال طلب عرض سعر إلا لشركة نشطة.");
            }

            if (req.CategoryId.HasValue)
            {
                _ = await uow.Category.GetByIdAsync(req.CategoryId.Value, ct)
                    ?? throw new NotFoundException(nameof(Category), req.CategoryId.Value);
            }

            var rfq = RfqRequest.Create(
                req.Title,
                req.Description,
                req.Quantity,
                cu.UserId.Value,
                req.Currency,
                req.UnitOfMeasure,
                req.SellerCompanyId,
                req.CategoryId,
                req.TargetPrice,
                req.ShippingCountry,
                req.DestinationCity,
                req.DestinationCountry,
                req.PreferredShippingMethod,
                req.PaymentTerms,
                req.RequiredCertifications,
                req.SupplierRequirements,
                req.DeadlineDate,
                req.ProductId,
                req.Attachments,
                req.IsPublic);

            await uow.RfqRequest.AddAsync(rfq, ct);
            await uow.SaveChangesAsync(ct);

            var full = await uow.RfqRequest.GetWithQuotesAsync(rfq.Id, ct);
            return ApiResponse<RfqRequestDto>.Created(mapper.Map<RfqRequestDto>(full));
        }

        public async Task<ApiResponse<RfqRequestDto>> Handle(CancelRfqCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var rfq = await uow.RfqRequest.GetWithQuotesAsync(req.RfqId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), req.RfqId);
            if (rfq.BuyerId != cu.UserId.Value) throw new ForbiddenException("Only the buyer can cancel.");
            rfq.Cancel();
            uow.RfqRequest.Update(rfq);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }

        public async Task<ApiResponse<RfqQuoteDto>> Handle(SubmitQuoteCommand req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null) throw new UnauthorizedException();

            var rfq = await uow.RfqRequest.GetWithQuotesAsync(req.RfqRequestId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), req.RfqRequestId);

            bool canQuote = rfq.IsPublic && !rfq.SellerCompanyId.HasValue
                || rfq.SellerCompanyId == cu.OwnedCompanyId.Value;

            if (!canQuote)
                throw new ForbiddenException("لا يمكن تقديم عرض سعر لهذا الطلب.");

            if (rfq.Status == RfqStatus.Cancelled || rfq.Status == RfqStatus.Accepted || rfq.Status == RfqStatus.Closed)
                throw new BusinessException($"Cannot quote on an RFQ with status '{rfq.Status}'.");

            var quote = RfqQuotation.Create(
                rfq.Id,
                cu.OwnedCompanyId.Value,
                req.UnitPrice,
                req.Quantity,
                req.Currency,
                req.Notes,
                req.PaymentTerms,
                req.DeliveryTerms,
                req.ValidityDays,
                req.LeadTimeDays,
                req.SampleAvailable);

            rfq.Quotes.Add(quote);
            rfq.MarkQuoted();
            uow.RfqRequest.Update(rfq);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<RfqQuoteDto>.Created(mapper.Map<RfqQuoteDto>(quote));
        }

        public async Task<ApiResponse<RfqRequestDto>> Handle(AcceptQuoteCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var quote = await uow.RfqQuotation.GetByIdAsync(req.QuoteId, ct)
                ?? throw new NotFoundException(nameof(RfqQuotation), req.QuoteId);

            var rfq = await uow.RfqRequest.GetWithQuotesAsync(quote.RfqRequestId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), quote.RfqRequestId);

            if (rfq.BuyerId != cu.UserId.Value)
                throw new ForbiddenException("Only the buyer can accept a quote.");

            if (quote.IsDeclined)
                throw new BusinessException("This quote has already been declined.");

            if (quote.IsExpired || quote.ValidUntil < DateTime.UtcNow)
                throw new BusinessException("This quote has expired and can no longer be accepted.");

            quote.Accept();
            rfq.MarkAccepted();

            uow.RfqQuotation.Update(quote);
            uow.RfqRequest.Update(rfq);

            await CreateOrderFromRfqAsync(rfq, quote, ct);

            await uow.SaveChangesAsync(ct);

            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }

        public async Task<ApiResponse<RfqRequestDto>> Handle(DeclineQuoteCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var quote = await uow.RfqQuotation.GetByIdAsync(req.QuoteId, ct)
                ?? throw new NotFoundException(nameof(RfqQuotation), req.QuoteId);

            var rfq = await uow.RfqRequest.GetWithQuotesAsync(quote.RfqRequestId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), quote.RfqRequestId);

            if (rfq.BuyerId != cu.UserId.Value)
                throw new ForbiddenException("Only the buyer can decline a quote.");

            if (quote.IsAccepted)
                throw new BusinessException("Cannot decline an already accepted quote.");

            if (quote.IsDeclined)
                throw new BusinessException("This quote has already been declined.");

            quote.Decline();
            uow.RfqQuotation.Update(quote);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }

        public async Task<ApiResponse<RfqRequestDto>> Handle(DeclineRfqCommand req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null) throw new UnauthorizedException();

            var rfq = await uow.RfqRequest.GetWithQuotesAsync(req.RfqId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), req.RfqId);

            if (!rfq.SellerCompanyId.HasValue || rfq.SellerCompanyId.Value != cu.OwnedCompanyId.Value)
                throw new ForbiddenException("Only the targeted seller company can decline this RFQ.");

            if (rfq.Status != RfqStatus.Pending)
                throw new BusinessException($"Cannot decline an RFQ with status '{rfq.Status}'.");

            rfq.MarkDeclined();
            uow.RfqRequest.Update(rfq);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }

        private async Task CreateOrderFromRfqAsync(RfqRequest rfq, RfqQuotation quote, CancellationToken ct)
        {
            var sellerCompany = await uow.Companies.GetByIdAsync(quote.SellerCompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), quote.SellerCompanyId);

            var order = Order.Create(rfq.BuyerId, rfq.Description, quote.Currency);

            var subOrder = OrderSubOrder.Create(
                order.Id,
                sellerCompany.Id,
                quote.Currency,
                quote.PaymentTerms,
                quote.Id);

            Guid productId;
            string productName;
            string? productImage = null;
            string? productDescription = null;
            string? categoryName = rfq.Category?.Name;

            if (rfq.ProductId.HasValue)
            {
                var product = await uow.Products.GetByIdAsync(rfq.ProductId.Value, ct)
                    ?? throw new NotFoundException(nameof(Product), rfq.ProductId.Value);
                productId = product.Id;
                productName = product.Name ?? rfq.Title;
                productImage = product.MainImageUrl;
                productDescription = product.Description;
            }
            else
            {
                var draftProduct = Product.Create(
                    rfq.Title,
                    rfq.Description,
                    sellerCompany.Id,
                    rfq.CategoryId ?? Guid.Empty,
                    rfq.Quantity,
                    quote.UnitPrice,
                    quote.Currency);
                draftProduct.UnitOfMeasure = rfq.UnitOfMeasure;
                await uow.Products.AddAsync(draftProduct, ct);
                await uow.SaveChangesAsync(ct);
                productId = draftProduct.Id;
                productName = draftProduct.Name;
                productDescription = draftProduct.Description;
            }

            var orderItem = OrderItem.Create(
                subOrder.Id,
                productId,
                productName,
                quote.Quantity,
                quote.UnitPrice,
                null,
                null,
                productImage,
                productDescription,
                categoryName,
                quote.UnitPrice,
                false,
                null,
                sellerCompany.CompanyName,
                null,
                quote.UnitPrice);

            subOrder.AddItem(orderItem);
            order.AddSubOrder(subOrder);

            await uow.Orders.AddAsync(order, ct);
        }
    }
}
