using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using E_Commerce.Data.Entity;

namespace E_Commerce.Core.Features.Orders.Queries.Handlers
{
    public class GetSellerOrdersHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetSellerOrdersQuery, ApiResponse<PaginatedResult<OrderSubOrderDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<OrderSubOrderDto>>> Handle(
            GetSellerOrdersQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.OwnedCompanyId == null)
                throw new ForbiddenException("لا تمتلك شركة.");

            var (items, total) = await uow.Orders.GetSubOrdersBySellerPagedAsync(
                cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);

            var dtos = mapper.Map<List<OrderSubOrderDto>>(items);
            var result = PaginatedResult<OrderSubOrderDto>.Success(dtos, total, req.Page, req.PageSize);
            return ApiResponse<PaginatedResult<OrderSubOrderDto>>.Ok(result);
        }
    }
}
