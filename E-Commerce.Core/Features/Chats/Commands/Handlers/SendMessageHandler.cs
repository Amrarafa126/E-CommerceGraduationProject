using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Core.Features.Chats.Hubs;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce.Core.Features.Chats.Commands.Handlers
{
    public class SendMessageHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IHubContext<ChatHub> hubContext,
        IMapper mapper)
        : IRequestHandler<SendMessageCommand, ApiResponse<MessageDto>>
    {
        public async Task<ApiResponse<MessageDto>> Handle(SendMessageCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var conversation = await uow.Conversations.GetByIdAsync(req.ConversationId, ct)
                ?? throw new NotFoundException(nameof(Conversation), req.ConversationId);

            var user = await uow.Users.GetByIdAsync(cu.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(User), cu.UserId.Value);

            bool isBuyer = conversation.BuyerId == cu.UserId.Value;
            bool isCompanyMember = user.OwnedCompanyId == conversation.CompanyId;

            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("You are not a participant in this conversation.");

            var messageType = (MessageType)(req.Type + 1);
            if (!Enum.IsDefined(typeof(MessageType), messageType))
                throw new BusinessException("Invalid message type.");

            var message = Message.Create(
                conversation.Id,
                cu.UserId.Value,
                req.Content,
                messageType,
                req.ReplyToMessageId,
                req.CardDataJson);

            // Add attachments
            if (req.Attachments?.Count > 0)
            {
                foreach (var att in req.Attachments)
                {
                    var attachmentType = (AttachmentType)(att.Type + 1);
                    if (!Enum.IsDefined(typeof(AttachmentType), attachmentType))
                        throw new BusinessException("Invalid attachment type.");

                    var attachment = MessageAttachment.Create(
                        message.Id, att.FileName, att.FileUrl,
                        attachmentType, att.FileSize, att.MimeType, att.DurationSeconds);
                    message.AddAttachment(attachment);
                }
            }

            await uow.Messages.AddAsync(message, ct);
            conversation.TouchLastMessage();
            uow.Conversations.Update(conversation);
            await uow.SaveChangesAsync(ct);

            // Use the in-memory message (already has attachments loaded)
            var dto = MapMessageDto(message, user);

            // Broadcast via SignalR
            var groupName = $"conv_{conversation.Id}";
            await hubContext.Clients.Group(groupName)
                .SendAsync("MessageReceived", new MessageReceivedDto
                {
                    ConversationId = conversation.Id,
                    Message = dto
                }, ct);

            return ApiResponse<MessageDto>.Created(dto);
        }

        private static MessageDto MapMessageDto(Message m, User sender) => new()
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            SenderName = sender.FullName,
            Content = m.Content,
            Type = (int)m.Type - 1,
            Attachments = m.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                Type = (int)a.Type - 1,
                FileSize = a.FileSize,
                DurationSeconds = a.DurationSeconds,
                MimeType = a.MimeType
            }).ToList(),
            ReplyToMessageId = m.ReplyToMessageId,
            CardDataJson = m.CardDataJson,
            ReadBy = m.ReadReceipts.Select(r => new ReadReceiptDto
            {
                UserId = r.UserId,
                UserName = r.User?.FullName ?? "",
                ReadAt = r.ReadAt
            }).ToList(),
            CreatedAt = m.CreatedAt
        };
    }
}
