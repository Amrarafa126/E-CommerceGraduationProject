using E_Commerce.Core.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core
{
    public static class ModuleCoreDependencies
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection service)
        {


            service.AddAutoMapper(Assembly.GetExecutingAssembly());


           service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            service.AddMediatR(cfg =>
            {
             // cfg.RegisterServicesFromAssembly(Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });
            // 
            // service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return service;
        }


    }
}
