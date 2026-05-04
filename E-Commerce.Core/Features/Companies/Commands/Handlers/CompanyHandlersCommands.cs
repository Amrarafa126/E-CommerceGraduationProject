using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.ValueObjects;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Companies.Commands.Handlers
{
    public class CompanyHandlersCommands(IUnitOfWork uow, ICurrentUserService cu, IMapper mapper) 
         : IRequestHandler<UpdateCompanyCommand, ApiResponse<CompanyDto>>
      
    {
        public async Task<ApiResponse<CompanyDto>> Handle(
       UpdateCompanyCommand req, CancellationToken ct)
        {
            var company = await uow.Companies.GetWithDetailsAsync(req.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Company), req.CompanyId);

            if (company.OwnerUserId != cu.UserId && cu.Role != "Admin")
                throw new ForbiddenException("Only the company owner or an Admin can update company info.");

            var address = new Address(req.Street, req.City, req.State, req.Country, req.PostalCode);
            var contact = new ContactInfo(req.ContactEmail, req.ContactPhone);
            company.Update(req.CompanyName, req.CompanyDescription, address, contact,
                req.YearEstablished, req.EmployeesCount);

            uow.Companies.Update(company);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<CompanyDto>.Ok(mapper.Map<CompanyDto>(company));
        }
      
    } 
}

