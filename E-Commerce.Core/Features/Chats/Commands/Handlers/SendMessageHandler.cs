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
using Microsoft.EntityFrameworkCore;

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
                throw new ForbiddenException("أنت لست مشاركاً في هذه المحادثة.");

            var messageType = (MessageType)(req.Type + 1);
            if (!Enum.IsDefined(typeof(MessageType), messageType))
                throw new BusinessException("نوع الرسالة غير صالح.");

            // ── Idempotency check ─────────────────────────────────────────
            // 1. Strong check: if client sent an IdempotencyKey, look it up directly
            if (!string.IsNullOrWhiteSpace(req.IdempotencyKey))
            {
                var existingByKey = await uow.Messages.FirstOrDefaultAsync(m =>
                    m.IdempotencyKey == req.IdempotencyKey, ct);

                if (existingByKey != null)
                {
                    var sender = await uow.Users.GetByIdAsync(existingByKey.SenderId, ct) ?? user;
                    var existingDto = MapMessageDto(existingByKey, sender);
                    return ApiResponse<MessageDto>.Ok(existingDto);
                }
            }

            // 2. Fallback check: same sender + same conversation + same content + same attachments count
            // within the last 5 seconds (catches rapid double-clicks / network retries without a key)
            var recentDuplicate = await uow.Messages.FirstOrDefaultAsync(m =>
                m.ConversationId == req.ConversationId &&
                m.SenderId == cu.UserId.Value &&
                m.Content == req.Content &&
                m.Type == messageType &&
                m.CreatedAt > DateTime.UtcNow.AddSeconds(-5), ct);

            if (recentDuplicate != null)
            {
                // Also verify attachments match (load them)
                var recentAttachments = await uow.MessageAttachments
                    .FindAsync(a => a.MessageId == recentDuplicate.Id, ct);
                var reqAttCount = req.Attachments?.Count ?? 0;
                if (recentAttachments.Count() == reqAttCount)
                {
                    var dupSender = await uow.Users.GetByIdAsync(recentDuplicate.SenderId, ct) ?? user;
                    var existingDto = MapMessageDto(recentDuplicate, dupSender);
                    return ApiResponse<MessageDto>.Ok(existingDto);
                }
            }

            var message = Message.Create(
                conversation.Id,
                cu.UserId.Value,
                req.Content,
                messageType,
                req.ReplyToMessageId,
                req.CardDataJson);

            // Store idempotency key on the message itself (DB unique constraint prevents true duplicates)
            if (!string.IsNullOrWhiteSpace(req.IdempotencyKey))
                message.SetIdempotencyKey(req.IdempotencyKey);

            // Add attachments
            if (req.Attachments?.Count > 0)
            {
                foreach (var att in req.Attachments)
                {
                    var attachmentType = (AttachmentType)(att.Type + 1);
                    if (!Enum.IsDefined(typeof(AttachmentType), attachmentType))
                        throw new BusinessException("نوع المرفق غير صالح.");

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
