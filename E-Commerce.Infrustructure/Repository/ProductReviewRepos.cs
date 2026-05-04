using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
 
namespace E_Commerce.Infrustructure.Repository
{
    public class ProductReviewRepos(AppDBContext Db) : GenericRepositoryAsync<ProductReview>(Db), IProductReviewRepos
    {
        public Task<ProductReview?> GetWithImagesAsync(Guid id, CancellationToken ct = default)
       => Db.productReviews
           .Include(r => r.Buyer).Include(r => r.Product).Include(r => r.Images)
           .FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<(IEnumerable<ProductReview> Items, int Total)> GetByProductAsync(
            Guid productId, int page, int pageSize, CancellationToken ct = default)
        {
            var q = Db.productReviews.AsNoTracking()
                .Include(r => r.Buyer).Include(r => r.Images)
                .Where(r => r.ProductId == productId && !r.IsDeleted);
            var total = await q.CountAsync(ct);
            var items = await q.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }

        public Task<bool> HasBuyerReviewedAsync(Guid productId, Guid buyerId, CancellationToken ct = default)
            => Db.productReviews.AnyAsync(
                r => r.ProductId == productId && r.BuyerId == buyerId && !r.IsDeleted, ct);
        public async Task<(double Avg, int Count, Dictionary<int, int> Dist)> GetStatsAsync(
            Guid productId, CancellationToken ct = default)
        {
            var ratings = await Db.productReviews
                .Where(r => r.ProductId == productId && !r.IsDeleted)
                .Select(r => r.Rating).ToListAsync(ct);

            if (!ratings.Any())
                return (0, 0, Enumerable.Range(1, 5).ToDictionary(i => i, _ => 0));

            return (Math.Round(ratings.Average(), 1), ratings.Count,
                Enumerable.Range(1, 5).ToDictionary(i => i, i => ratings.Count(r => r == i)));
        }
    }
}
