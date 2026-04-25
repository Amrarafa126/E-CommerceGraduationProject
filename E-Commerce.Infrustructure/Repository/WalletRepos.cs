using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class WalletRepos : GenericRepositoryAsync<Wallet>, IWalletRepos
    {
        DbSet<Wallet> wallets;

        public WalletRepos(AppDBContext context) : base(context)
        {
            wallets = context.Wallets;
        }
    }
}
