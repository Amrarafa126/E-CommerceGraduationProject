using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Chats.Commands.Handlers
{
    public class StartConversationHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IMapper mapper)
        : IRequestHandler<StartConversationCommand, ApiResponse<ConversationDto>>
    {
        public async Task<ApiResponse<ConversationDto>> Handle(StartConversationCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var company = await uow.Companies.GetByIdAsync(req.CompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.CompanyId);

            var existing = await uow.Conversations.FindBetweenAsync(
                cu.UserId.Value, req.CompanyId, ct);

            Conversation conversation;
            if (existing != null)
            {
                conversation = existing;
                // Unarchive if previously archived so it reappears in both lists
                bool wasArchived = conversation.IsBuyerArchived || conversation.IsCompanyArchived;
                if (conversation.IsBuyerArchived)
                    conversation.UnarchiveForBuyer();
                if (conversation.IsCompanyArchived)
                    conversation.UnarchiveForCompany();
                if (wasArchived)
                    uow.Conversations.Update(conversation);
            }
            else
            {
                conversation = Conversation.Create(cu.UserId.Value, req.CompanyId, req.Subject);
                if (req.RelatedProductId.HasValue) conversation.SetRelatedProduct(req.RelatedProductId.Value);
                if (req.RelatedOrderId.HasValue) conversation.SetRelatedOrder(req.RelatedOrderId.Value);
                if (req.RelatedRfqId.HasValue) conversation.SetRelatedRfq(req.RelatedRfqId.Value);

                await uow.Conversations.AddAsync(conversation, ct);
                await uow.SaveChangesAsync(ct);
            }

            var message = Message.Create(conversation.Id, cu.UserId.Value, req.InitialMessage);
            await uow.Messages.AddAsync(message, ct);
            conversation.TouchLastMessage();
            uow.Conversations.Update(conversation);
            await uow.SaveChangesAsync(ct);

            var buyer = await uow.Users.GetByIdAsync(cu.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(User), cu.UserId.Value);

            return ApiResponse<ConversationDto>.Created(MapConversation(conversation, buyer, company, 1, message));
        }

        private static ConversationDto MapConversation(
            Conversation c, User buyer, Company company,
            int unread, Message? lastMsg) => new()
        {
            Id = c.Id,
            BuyerId = c.BuyerId,
            BuyerName = buyer.FullName,
            BuyerEmail = buyer.Email,
            CompanyId = c.CompanyId,
            CompanyName = company.CompanyName ?? "",
            CompanyLogoUrl = company.LogoUrl,
            Subject = c.Subject,
            UnreadCount = unread,
            LastMessage = lastMsg == null ? null : new MessageDto
            {
                Id = lastMsg.Id,
                ConversationId = lastMsg.ConversationId,
                SenderId = lastMsg.SenderId,
                SenderName = buyer.FullName,
                Content = lastMsg.Content,
                Type = (int)lastMsg.Type - 1,
                CreatedAt = lastMsg.CreatedAt
            },
            LastMessageAt = c.LastMessageAt,
            CreatedAt = c.CreatedAt,
            RelatedProductId = c.RelatedProductId,
            RelatedOrderId = c.RelatedOrderId,
            RelatedRfqId = c.RelatedRfqId
        };
    }
}
