using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Orders.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Orders.Queries.Handlers
{
    public class GetMyOrdersHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetMyOrdersQuery, ApiResponse<PaginatedResult<OrderDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<OrderDto>>> Handle(
            GetMyOrdersQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var (items, total) = await uow.Orders.GetByBuyerPagedAsync(
                cu.UserId.Value, req.Page, req.PageSize, ct);

            var dtos = mapper.Map<List<OrderDto>>(items);
            var result = PaginatedResult<OrderDto>.Success(dtos, total, req.Page, req.PageSize);
            return ApiResponse<PaginatedResult<OrderDto>>.Ok(result);
        }
    }
}
