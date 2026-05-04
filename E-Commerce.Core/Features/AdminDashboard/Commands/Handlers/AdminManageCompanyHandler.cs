using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.AdminDashboard.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.AdminDashboard.Commands.Handlers
{
    public class AdminManageCompanyHandler(IUnitOfWork uow, ICurrentUserService cu)
     : IRequestHandler<AdminManageCompanyCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(AdminManageCompanyCommand req, CancellationToken ct)
        {
            if (cu.Role != "Admin") throw new ForbiddenException("Admin only.");

            var company = await uow.Companies.GetByIdAsync(req.CompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.CompanyId);
            switch (req.Action)
            {
                case CompanyAdminAction.Approve:
                    company.Approve();
                    break;
                case CompanyAdminAction.Reject:
                    company.Reject();
                    break;
                case CompanyAdminAction.Suspend:
                    company.Suspend();
                    break;
                case CompanyAdminAction.Reactivate:
                    company.Approve();
                    break;
            }
            uow.Companies.Update(company);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok($"Company {req.Action} applied.");
        }
    }
}
