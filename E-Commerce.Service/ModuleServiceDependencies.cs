using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection service)
        {
            service.AddTransient<ICategoryService, CategoryService>();
            return service;
        }

    }
}
