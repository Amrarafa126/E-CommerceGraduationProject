using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Queries.Handlers
{
    public class GetPayoutsHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetPayoutsQuery, ApiResponse<PaginatedResult<PayoutDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<PayoutDto>>> Handle(
            GetPayoutsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("Only Sellers can view payouts.");

            var (items, total) = await uow.payout.GetByCompanyPagedAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);

            var dtos = mapper.Map<List<PayoutDto>>(items);
            var result = PaginatedResult<PayoutDto>.Success(dtos, total, req.Page, req.PageSize);
            return ApiResponse<PaginatedResult<PayoutDto>>.Ok(result);
        }
    }
}
