using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Queries.Handlers
{
    public class GetAllPayoutsHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetAllPayoutsQuery, ApiResponse<PaginatedResult<PayoutDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<PayoutDto>>> Handle(
            GetAllPayoutsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.Role != "Admin")
                throw new ForbiddenException("يمكن للمسؤولين فقط عرض جميع طلبات السحب.");

            var (items, total) = await uow.payout.GetAllPagedAsync(
                req.Status, req.Page, req.PageSize, ct);

            var dtos = mapper.Map<List<PayoutDto>>(items);
            var result = PaginatedResult<PayoutDto>.Success(dtos, total, req.Page, req.PageSize);
            return ApiResponse<PaginatedResult<PayoutDto>>.Ok(result);
        }
    }
}
