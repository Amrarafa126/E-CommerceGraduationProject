using MediatR;

namespace E_Commerce.Core.Features.Chats.Commands.Models
{
    public record StartConversationCommand(
        Guid CompanyId,
        string InitialMessage,
        string? Subject = null,
        Guid? RelatedProductId = null,
        Guid? RelatedOrderId = null,
        Guid? RelatedRfqId = null)
        : IRequest<ApiResponse<ConversationDto>>;
}
