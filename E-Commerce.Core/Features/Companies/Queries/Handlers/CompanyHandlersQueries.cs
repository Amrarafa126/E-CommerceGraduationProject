using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Companies.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Companies.Queries.Handlers
{
    public class CompanyHandlersQueries(IUnitOfWork uow, IMapper mapper, ICurrentUserService cu)
    : IRequestHandler<GetCompanyByIdQuery, ApiResponse<CompanyDto>>
    {
        public async Task<ApiResponse<CompanyDto>> Handle(GetCompanyByIdQuery req, CancellationToken ct)
        {
            var company = await uow.Companies.GetWithDetailsAsync(req.CompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.CompanyId);
            return ApiResponse<CompanyDto>.Ok(mapper.Map<CompanyDto>(company));
        }

        public async Task<ApiResponse<CompanyDto>> Handle(GetMyCompanyQuery req, CancellationToken ct)
        {
            if (cu.OwnedCompanyId == null) throw new BusinessException("You do not own a company.");
            var company = await uow.Companies.GetWithDetailsAsync(cu.OwnedCompanyId.Value, ct)
                ?? throw new NotFoundException(nameof(Company), cu.OwnedCompanyId.Value);
            return ApiResponse<CompanyDto>.Ok(mapper.Map<CompanyDto>(company));
        }

        public async Task<ApiResponse<PaginatedResult<CompanySummaryDto>>> Handle(
        GetCompaniesQuery req, CancellationToken ct)
        {
            var (items, total) = await uow.Companies.GetPagedAsync(req.Page, req.PageSize, req.Search, ct);
            var dtos = mapper.Map<IEnumerable<CompanySummaryDto>>(items);
            return ApiResponse<PaginatedResult<CompanySummaryDto>>.Ok(
                PaginatedResult<CompanySummaryDto>.Success(dtos, total, req.Page, req.PageSize));
        }
    }
}
