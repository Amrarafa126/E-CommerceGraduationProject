using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IProductReviewRepos : IGenericRepositoryAsync<ProductReview>
    {
        Task<ProductReview?> GetWithImagesAsync(Guid reviewId, CancellationToken ct = default);
        Task<(IEnumerable<ProductReview> Items, int Total)> GetByProductAsync(
            Guid productId, int page, int pageSize, CancellationToken ct = default);
        Task<bool> HasBuyerReviewedAsync(Guid productId, Guid buyerId, CancellationToken ct = default);
        Task<(double Avg, int Count, Dictionary<int, int> Dist)> GetStatsAsync(
            Guid productId, CancellationToken ct = default);
    }
}
