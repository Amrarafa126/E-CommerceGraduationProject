using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Payment.Queries.Models;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Payment.Queries.Handlers
{
    public class GetWalletHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<GetWalletQuery, ApiResponse<WalletDto>>
    {
        public async Task<ApiResponse<WalletDto>> Handle(GetWalletQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            if (cu.Role != "Seller" || cu.OwnedCompanyId == null)
                throw new ForbiddenException("المحافظ متاحة للبائعين فقط.");

            var wallet = await uow.Wallets.GetByCompanyAsync(cu.OwnedCompanyId.Value, ct)
                ?? throw new NotFoundException("محفظة الشركة", cu.OwnedCompanyId.Value);

            var dto = mapper.Map<WalletDto>(wallet);
            return ApiResponse<WalletDto>.Ok(dto);
        }
    }
}
