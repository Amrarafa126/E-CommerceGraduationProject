using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Payment.Queries.Models
{
    public record GetWalletQuery : IRequest<ApiResponse<WalletDto>>;

}
