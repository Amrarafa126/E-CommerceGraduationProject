using E_Commerce.Data.Entity;
using E_Commerce.Service.Interfase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Repostoiry
{

    public class VariantService : IVariantService
    {
        public List<ProductVariant> BuildVariants(Product product)
        {
            // 🟢 Simple Product (no options)
            //if (product.ProductOptions == null || !product.ProductOptions.Any())
            //{
            //    if (product.productVariants != null && product.productVariants.Any())
            //        return new List<ProductVariant>();

            //    return new List<ProductVariant>
            //{
            //    new ProductVariant
            //    {
            //        SKU = GenerateSku(product.Id),
            //        ProductId = product.Id,
            //        Price = 0,
            //        StockQuantity= 0
            //    }
            //};
            //}

            // 🟢 Ensure not null
            if (product.productVariants == null)
                product.productVariants = new List<ProductVariant>();

            var optionGroups = product.ProductOptions
                .Where(o => o.Values != null && o.Values.Any())
                .Select(o => o.Values!.ToList())
                .ToList();

            if (!optionGroups.Any())
                return new List<ProductVariant>();

            var combinations = GenerateCombinations(optionGroups);

            var existingSignatures = product.productVariants
                .Where(v => v.VariantValues != null)
                .Select(v => GenerateSignature(
                    v.VariantValues.Select(x => x.ProductOptionValueId).ToList()
                ))
                .ToHashSet();

            var variants = new List<ProductVariant>();

            foreach (var combo in combinations)
            {
                var ids = combo.Select(x => x.Id).ToList();
                var signature = GenerateSignature(ids);

                if (existingSignatures.Contains(signature))
                    continue;

                variants.Add(new ProductVariant
                {
                    SKU = GenerateSku(product.Id),
                    ProductId = product.Id,
                    VariantValues = combo.Select(v => new ProductVariantValue
                    {
                        ProductOptionValueId = v.Id
                    }).ToList()
                });
            }

            return variants;
        }

        // 🔥 combinations
        private List<List<ProductOptionValue>> GenerateCombinations(
            List<List<ProductOptionValue>> options)
        {
            var result = new List<List<ProductOptionValue>>();

            void Backtrack(int depth, List<ProductOptionValue> current)
            {
                if (depth == options.Count)
                {
                    result.Add(new List<ProductOptionValue>(current));
                    return;
                }

                foreach (var value in options[depth])
                {
                    current.Add(value);
                    Backtrack(depth + 1, current);
                    current.RemoveAt(current.Count - 1);
                }
            }

            Backtrack(0, new List<ProductOptionValue>());

            return result;
        }

        // 🔥 duplicate prevention
        private string GenerateSignature(List<int> ids)
        {
            return string.Join("-", ids.OrderBy(x => x));
        }

        private string GenerateSku(int productId)
        {
            return $"PRD-{productId}-{Guid.NewGuid().ToString().Substring(0, 6)}";
        }
    }
}





