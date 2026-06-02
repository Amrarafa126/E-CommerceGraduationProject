using MediatR;

namespace E_Commerce.Core.Features.ReviewHelpfulVotes.Commands.Models
{
    public record ToggleReviewHelpfulVoteCommand(Guid ReviewId)
        : IRequest<ApiResponse<object>>;
}
