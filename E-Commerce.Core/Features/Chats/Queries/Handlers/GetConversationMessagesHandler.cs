using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Queries.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;

namespace E_Commerce.Core.Features.Chats.Queries.Handlers
{
    public class GetConversationMessagesHandler(
        IUnitOfWork uow,
        ICurrentUserService cu)
        : IRequestHandler<GetConversationMessagesQuery, ApiResponse<ConversationPageDto>>
    {
        public async Task<ApiResponse<ConversationPageDto>> Handle(
            GetConversationMessagesQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var conversation = await uow.Conversations.GetWithMessagesAsync(
                req.ConversationId, req.Page, req.PageSize, ct)
                ?? throw new NotFoundException(nameof(Conversation), req.ConversationId);

            bool isBuyer = conversation.BuyerId == cu.UserId.Value;
            bool isCompanyMember = cu.OwnedCompanyId == conversation.CompanyId;

            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("Access denied to this conversation.");

            var messageDtos = conversation.Messages
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.FullName ?? "",
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
                    ReplyToMessage = m.ReplyToMessage == null ? null : new MessageDto
                    {
                        Id = m.ReplyToMessage.Id,
                        ConversationId = m.ReplyToMessage.ConversationId,
                        SenderId = m.ReplyToMessage.SenderId,
                        SenderName = m.ReplyToMessage.Sender?.FullName ?? "",
                        Content = m.ReplyToMessage.Content,
                        Type = (int)m.ReplyToMessage.Type - 1,
                        CreatedAt = m.ReplyToMessage.CreatedAt
                    },
                    CardDataJson = m.CardDataJson,
                    ReadBy = m.ReadReceipts.Select(r => new ReadReceiptDto
                    {
                        UserId = r.UserId,
                        UserName = r.User?.FullName ?? "",
                        ReadAt = r.ReadAt
                    }).ToList(),
                    CreatedAt = m.CreatedAt
                })
                .ToList();

            var convDto = new ConversationDto
            {
                Id = conversation.Id,
                BuyerId = conversation.BuyerId,
                BuyerName = conversation.Buyer?.FullName ?? "",
                BuyerEmail = conversation.Buyer?.Email,
                CompanyId = conversation.CompanyId,
                CompanyName = conversation.Company?.CompanyName ?? "",
                CompanyLogoUrl = conversation.Company?.LogoUrl,
                Subject = conversation.Subject,
                UnreadCount = 0,
                LastMessage = messageDtos.FirstOrDefault(),
                LastMessageAt = conversation.LastMessageAt,
                CreatedAt = conversation.CreatedAt,
                RelatedProductId = conversation.RelatedProductId,
                RelatedOrderId = conversation.RelatedOrderId,
                RelatedRfqId = conversation.RelatedRfqId
            };

            var totalMessages = await uow.Messages.CountAsync(
                m => m.ConversationId == req.ConversationId, ct);

            return ApiResponse<ConversationPageDto>.Ok(
                new ConversationPageDto
                {
                    Conversation = convDto,
                    Messages = messageDtos,
                    TotalMessages = totalMessages,
                    Page = req.Page,
                    PageSize = req.PageSize
                });
        }
    }
}
