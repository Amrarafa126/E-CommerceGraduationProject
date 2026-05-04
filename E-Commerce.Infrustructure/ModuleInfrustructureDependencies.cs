using E_Commerce.Infrustructure.ImpelmationUnitOfWork;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Infrustructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure
{
    public static class ModuleInfrustructureDependencies
    {
        public static IServiceCollection AddInfrustructureDependencies(this IServiceCollection service)
        {
            service.AddTransient<ICategoryRepos,CategoryRepos>();
            service.AddTransient<IProductRepos,   ProductRepos>();
            service.AddTransient<ICompanyRepos,     CompanyRepos>();
            service.AddTransient<IUserRepos, UserRepos>();
            service.AddTransient<IWalletRepos, WalletRepos>();
            service.AddTransient<IOrderRepos, OrderRepos>();
            service.AddTransient<IOrderItemRepos, OrderItemRepos>();
            service.AddTransient<IPaymentRepos, PaymentRepos>();
            service.AddTransient<IShippingRepos, ShippingRepos>();
            service.AddTransient<IMessageRepos, MessageRepos>();
            service.AddTransient<IRfqQuotationRepos, RfqQuotationRepos>();
            service.AddTransient<IRfqRequestRepos, RfqRequestRepos>();
            service.AddTransient<IUnitOfWork, UnitOfWork>();

            service.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));

            return service;
        }


    }
}
