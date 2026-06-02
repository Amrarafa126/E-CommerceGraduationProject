using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.RFQ.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _ = await uow.Companies.GetByIdAsync(req.SellerCompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.SellerCompanyId);

            var rfq = RfqRequest.Create(req.Title, req.Description, req.Quantity,
                cu.UserId.Value, req.SellerCompanyId, req.Currency,
                req.TargetPrice, req.ShippingCountry, req.DeadlineDate, req.ProductId, req.Attachments);

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

            if (cu.OwnedCompanyId == null || cu.OwnedCompanyId != rfq.SellerCompanyId)
                throw new ForbiddenException("Only the seller company can submit quotes.");

            if (rfq.Status == RfqStatus.Cancelled || rfq.Status == RfqStatus.Accepted)
                throw new BusinessException($"Cannot quote on an RFQ with status '{rfq.Status}'.");

            var quote = RfqQuotation.Create(rfq.Id, req.UnitPrice, req.Quantity,
                req.Currency, req.Notes, req.PaymentTerms, req.DeliveryTerms, req.ValidityDays);

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

            if (cu.OwnedCompanyId != rfq.SellerCompanyId)
                throw new ForbiddenException("Only the targeted seller company can decline this RFQ.");

            if (rfq.Status != RfqStatus.Pending)
                throw new BusinessException($"Cannot decline an RFQ with status '{rfq.Status}'.");

            rfq.MarkDeclined();
            uow.RfqRequest.Update(rfq);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }
    }
}
