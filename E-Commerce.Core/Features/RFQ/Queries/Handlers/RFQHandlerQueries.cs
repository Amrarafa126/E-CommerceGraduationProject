using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.RFQ.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;


namespace E_Commerce.Core.Features.RFQ.Queries.Handlers
{
    public class RFQHandlerQueries(IUnitOfWork uow, IMapper mapper , ICurrentUserService cu)
    : IRequestHandler<GetRfqByIdQuery, ApiResponse<RfqRequestDto>>,
      IRequestHandler<GetMyRfqsQuery, ApiResponse<PaginatedResult<RfqRequestDto>>>,
      IRequestHandler<GetSellerRfqsQuery, ApiResponse<PaginatedResult<RfqRequestDto>>>
    {
        public async Task<ApiResponse<RfqRequestDto>> Handle(GetRfqByIdQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var rfq = await uow.RfqRequest.GetWithQuotesAsync(req.RfqId, ct)
                ?? throw new NotFoundException(nameof(RfqRequest), req.RfqId);

            bool isBuyer = rfq.BuyerId == cu.UserId.Value;
            bool isSeller = cu.OwnedCompanyId != null && rfq.SellerCompanyId == cu.OwnedCompanyId.Value;
            bool isAdmin = cu.Role == "Admin";

            if (!isBuyer && !isSeller && !isAdmin)
                throw new ForbiddenException("You are not authorized to view this RFQ.");

            return ApiResponse<RfqRequestDto>.Ok(mapper.Map<RfqRequestDto>(rfq));
        }
        public async Task<ApiResponse<PaginatedResult<RfqRequestDto>>> Handle(
       GetMyRfqsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();
            var (items, total) = await uow.RfqRequest.GetByBuyerPagedAsync(cu.UserId.Value, req.Page, req.PageSize, ct);
            var dtos = mapper.Map<IEnumerable<RfqRequestDto>>(items);
            return ApiResponse<PaginatedResult<RfqRequestDto>>.Ok(
                PaginatedResult<RfqRequestDto>.Success(dtos, total, req.Page, req.PageSize));
        }

        public async Task<ApiResponse<PaginatedResult<RfqRequestDto>>> Handle(
      GetSellerRfqsQuery req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null) throw new ForbiddenException("Only sellers can view incoming RFQs.");
            var (items, total) = await uow.RfqRequest.GetBySellerPagedAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, req.Status, ct);
            var dtos = mapper.Map<IEnumerable<RfqRequestDto>>(items);
            return ApiResponse<PaginatedResult<RfqRequestDto>>.Ok(
                PaginatedResult<RfqRequestDto>.Success(dtos, total, req.Page, req.PageSize));
        }

    }
}
