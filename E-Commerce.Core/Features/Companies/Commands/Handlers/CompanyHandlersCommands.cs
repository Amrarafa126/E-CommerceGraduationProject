using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Core.BaseResponse;
using E_Commerce.Core.Features.Companies.Commands.Models;
using E_Commerce.Core.Mapping.Companies;
using E_Commerce.Data.Entity;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using MediatR;

namespace E_Commerce.Core.Features.Companies.Commands.Handlers
{
    public class CompanyHandlersCommands : ResponseHandler, IRequestHandler<AddCompanyCommands, Response<string>>
    {
        IMapper mapper;
        ICompanyService CompanyService;
        public CompanyHandlersCommands(IMapper mapper, ICompanyService CompanyService)
        {
            this.mapper = mapper;
            this.CompanyService = CompanyService;
        }
        public async Task<Response<string>> Handle(AddCompanyCommands request, CancellationToken cancellationToken)
        {
            var CampanyMapper = mapper.Map<Company>(request);

            var result = await CompanyService.AddCategoryAsync(CampanyMapper);
            if (result == null)
                return UnprocessableEntity<string>();
            return Success(result);

        }
    }
}
