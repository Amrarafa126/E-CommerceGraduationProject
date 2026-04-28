using AutoMapper;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.RFQ.Commands.Models
{
    public record AcceptQuoteCommand(Guid QuoteId) : IRequest<ApiResponse<RfqRequestDto>>;
    
}
