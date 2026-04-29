using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection service)
        {
        //    service.AddTransient<ICategoryService, CategoryService>();
        //    service.AddTransient<IProductService, ProductService>();
        //    service.AddTransient<IFileService, FileService>();
        //    service.AddTransient<ICompanyService, CompanyService>();
        //    service.AddTransient<IProductImageService , ProductImageService>();
        //    service.AddTransient<IProductOptionService, ProductOptionService>();
        //    service.AddTransient<IProductOptionValueService, ProductOptionValueService>();
            //service.AddTransient<IProductPriceTierService , ProductPriceTierService>();
            service.AddTransient<IApplicationUserService, ApplicationUserService>();
            service.AddTransient<IEmailsService, EmailsService>();
            service.AddTransient<IAuthenticationService, AuthenticationService>();
            //service.AddTransient<IVariantService, VariantService>();
            service.AddTransient<ICurrentUserService, CurrentUserService>();
            service.AddSingleton<ITokenService, JwtTokenService>();
            service.AddTransient<IFileStorageService, LocalFileStorageService>();

            return service;
        }

    }
}
