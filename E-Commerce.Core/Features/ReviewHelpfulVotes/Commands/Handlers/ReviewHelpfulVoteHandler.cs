using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.ReviewHelpfulVotes.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.ReviewHelpfulVotes.Commands.Handlers
{
    public class ReviewHelpfulVoteHandler(IUnitOfWork uow, ICurrentUserService cu)
        : IRequestHandler<ToggleReviewHelpfulVoteCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(ToggleReviewHelpfulVoteCommand req, CancellationToken ct)
        {
            if (cu.UserId == null) throw new UnauthorizedException();

            var review = await uow.Reviews.GetByIdAsync(req.ReviewId, ct)
                ?? throw new NotFoundException(nameof(ProductReview), req.ReviewId);

            var existing = await uow.ReviewHelpfulVotes
                .FirstOrDefaultAsync(hv => hv.ReviewId == req.ReviewId && hv.ReviewerId == cu.UserId.Value, ct);

            if (existing != null)
            {
                // Remove vote
                uow.ReviewHelpfulVotes.Remove(existing);
                review.DecrementHelpful();
            }
            else
            {
                // Add vote
                var vote = ReviewHelpfulVote.Create(req.ReviewId, cu.UserId.Value);
                await uow.ReviewHelpfulVotes.AddAsync(vote, ct);
                review.IncrementHelpful();
            }

            uow.Reviews.Update(review);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<object>.Ok(existing != null ? "Vote removed." : "Vote added.");
        }
    }
}
