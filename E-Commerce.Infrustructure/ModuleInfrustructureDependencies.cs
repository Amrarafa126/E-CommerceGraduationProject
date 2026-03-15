using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Infrustructure
{
    public static class ModuleInfrustructureDependencies
    {
        public static IServiceCollection AddInfrustructureDependencies(this IServiceCollection service)
        {
            service.AddTransient<ICategoryRepos, CategoryRepos>();
            service.AddTransient<IProductRepos, ProductRepos>();

          
            service.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            return service;
        }


    }
}
