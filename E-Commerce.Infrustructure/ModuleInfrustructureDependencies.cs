using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace E_Commerce.Infrustructure
{
    public static class ModuleInfrustructureDependencies
    {
        public static IServiceCollection AddInfrustructureDependencies(this IServiceCollection service)
        {
            service.AddTransient<ICategoryRepos,CategoryRepos>();
            service.AddTransient<IProductRepos,   ProductRepos>();
            service.AddTransient<ICompanyRepos,     CompanyRepos>();
            service.AddTransient<IProductImageRepos,   ProductImageRepos>();
            service.AddTransient<IProductOptionsRepos,   ProductOptionsRepos>();
            service.AddTransient<IProductOptionValuesRepos, ProductOptionValuesRepos>();
            service.AddTransient<IProductPriceTierRepos     , ProductPriceTierRepos>();
            service.AddTransient<IRefreshTokenRepository      , RefreshTokenRepository>();

            service.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            return service;
        }


    }
}
