using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Service.Interfase;
using E_Commerce.Service.Repostoiry;
using E_Commerce.Service.Payment;
using E_Commerce.Service.Shipping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection service)
        {
            service.AddTransient<IEmailsService, EmailsService>();
            service.AddTransient<ICurrentUserService, CurrentUserService>();
            service.AddSingleton<ITokenService, JwtTokenService>();
            service.AddTransient<IGoogleAuthService, GoogleAuthService>();
            service.AddTransient<IFileStorageService, LocalFileStorageService>();
            service.AddTransient<IPaymentGateway, PaymobPaymentGateway>();
            service.AddTransient<IBostaShippingService, BostaShippingService>();
            service.AddHttpClient<IPaymobClient, PaymobClient>();
            service.AddHttpClient<IBostaClient, BostaClient>();

            return service;
        }
    }
}
