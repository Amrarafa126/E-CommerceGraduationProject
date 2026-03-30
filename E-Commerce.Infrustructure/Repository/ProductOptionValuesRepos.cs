using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrustructure.Repository
{
    public class ProductOptionValuesRepos : GenericRepositoryAsync<ProductOptionValue>,IProductOptionValuesRepos
    {
        DbSet<ProductOptionValue> productOptionValues;

        public ProductOptionValuesRepos(AppDBContext context) : base(context)
        {
            productOptionValues = context.Set<ProductOptionValue>();
        }
    }
}
