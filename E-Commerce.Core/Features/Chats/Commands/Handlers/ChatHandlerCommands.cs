using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Features.Chats.Commands.Handlers
{
    public class ChatHandlerCommands(
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<StartConversationCommand, ApiResponse<ConversationDto>>,
        IRequestHandler<SendMessageCommand, ApiResponse<MessageDto>>,
         IRequestHandler<MarkMessagesReadCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<ConversationDto>> Handle(
        StartConversationCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var company = await uow.Companies.GetByIdAsync(req.CompanyId, ct)
                ?? throw new NotFoundException(nameof(Company), req.CompanyId);

            // Re-use existing conversation if one already exists
            var existing = await uow.Conversations.FindBetweenAsync(
                currentUser.UserId.Value, req.CompanyId, ct);

            Conversation conversation;
            if (existing != null)
            {
                conversation = existing;
            }
            else
            {
                conversation = Conversation.Create(currentUser.UserId.Value, req.CompanyId);
                await uow.Conversations.AddAsync(conversation, ct);
                await uow.SaveChangesAsync(ct);
            }

            // Add the first message
            var message = Message.Create(conversation.Id, currentUser.UserId.Value, req.InitialMessage);
            await uow.Messages.AddAsync(message, ct);
            conversation.TouchLastMessage();
            uow.Conversations.Update(conversation);
            await uow.SaveChangesAsync(ct);

            var buyer = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct);
            return ApiResponse<ConversationDto>.Created(MapConversation(conversation, buyer!, company, 0, message));
        }

        private static ConversationDto MapConversation(
            Conversation c, User buyer, Company company,
            int unread, Message? lastMsg) => new(
            c.Id, c.BuyerId, buyer.FullName, buyer.Email,
            c.CompanyId, company.CompanyName, company.LogoUrl,
            unread,
            lastMsg == null ? null : new MessageDto(
                lastMsg.Id, lastMsg.ConversationId, lastMsg.SenderId,
                buyer.FullName, lastMsg.Content, lastMsg.AttachmentUrl,
                lastMsg.Type.ToString(), lastMsg.IsRead, lastMsg.ReadAt, lastMsg.CreatedAt),
            c.LastMessageAt, c.CreatedAt);
    
     public async Task<ApiResponse<MessageDto>> Handle(SendMessageCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var conversation = await uow.Conversations.GetByIdAsync(req.ConversationId, ct)
                ?? throw new NotFoundException(nameof(Conversation), req.ConversationId);

            // Validate sender is part of this conversation
            var user = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct)!
                ?? throw new NotFoundException(nameof(User), currentUser.UserId.Value);

            bool isBuyer = conversation.BuyerId == currentUser.UserId.Value;
            bool isCompanyMember = user.OwnedCompanyId == conversation.CompanyId;

            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("You are not a participant in this conversation.");

            var type = Enum.TryParse<MessageType>(req.Type, true, out var parsed)
                ? parsed : MessageType.Text;

            var message = Message.Create(
                conversation.Id, currentUser.UserId.Value, req.Content, req.AttachmentUrl, type);

            await uow.Messages.AddAsync(message, ct);
            conversation.TouchLastMessage();
            uow.Conversations.Update(conversation);
            await uow.SaveChangesAsync(ct);

            return ApiResponse<MessageDto>.Created(new MessageDto(
                message.Id, message.ConversationId, message.SenderId,
                user.FullName, message.Content, message.AttachmentUrl,
                message.Type.ToString(), message.IsRead, message.ReadAt, message.CreatedAt));
        }

        public async Task<ApiResponse<object>> Handle(MarkMessagesReadCommand req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();
            await uow.Messages.MarkAllReadAsync(req.ConversationId, currentUser.UserId.Value, ct);
            await uow.SaveChangesAsync(ct);
            return ApiResponse<object>.Ok("Messages marked as read.");
        }

    }
}
