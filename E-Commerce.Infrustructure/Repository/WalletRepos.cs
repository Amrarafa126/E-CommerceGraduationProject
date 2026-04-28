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
    public class WalletRepos(AppDBContext Db) : GenericRepositoryAsync<Wallet>(Db), IWalletRepos
    {
        public Task<Wallet?> GetByCompanyAsync(Guid companyId, CancellationToken ct = default)
       => Db.Wallets.FirstOrDefaultAsync(w => w.CompanyId == companyId, ct);
    }
}
