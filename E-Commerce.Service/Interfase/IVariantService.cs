using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfase
{
   
        public interface IVariantService
        {
            List<ProductVariant> BuildVariants(Product product);
        }
}

