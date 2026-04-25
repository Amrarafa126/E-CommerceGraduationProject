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
    public class ProductVariantValueRepos : GenericRepositoryAsync<ProductVariantOptionValue>, IProductVariantValueRepos
    {
        DbSet<ProductOptionValuesRepos> productVariantValues;
        public ProductVariantValueRepos(AppDBContext context) : base(context)
        {
            productVariantValues = context.Set<ProductOptionValuesRepos>();
        }
    }
}
