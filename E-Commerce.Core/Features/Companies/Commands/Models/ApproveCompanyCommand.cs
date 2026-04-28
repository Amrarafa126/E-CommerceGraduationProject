using MediatR;


namespace E_Commerce.Core.Features.Companies.Commands.Models
{
    public record ApproveCompanyCommand(Guid CompanyId) : IRequest<ApiResponse<CompanyDto>>;

}
