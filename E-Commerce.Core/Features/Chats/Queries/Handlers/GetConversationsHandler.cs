using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Queries.Models;
using E_Commerce.Core.Wrappers;
using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Core.Features.Chats.Queries.Handlers
{
    public class GetConversationsHandler(
        IUnitOfWork uow,
        ICurrentUserService cu)
        : IRequestHandler<GetConversationsQuery, ApiResponse<PaginatedResult<ConversationDto>>>
    {
        public async Task<ApiResponse<PaginatedResult<ConversationDto>>> Handle(
            GetConversationsQuery req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            IEnumerable<Conversation> items;
            int total;

            if (cu.OwnedCompanyId != null)
            {
                (items, total) = await uow.Conversations.GetByCompanyAsync(
                    cu.OwnedCompanyId.Value, req.Page, req.PageSize, ct);
            }
            else
            {
                (items, total) = await uow.Conversations.GetByBuyerAsync(
                    cu.UserId.Value, req.Page, req.PageSize, ct);
            }

            var conversationIds = items.Select(c => c.Id).ToList();
            var unreadCounts = await uow.Conversations.CountUnreadBatchAsync(
                conversationIds, cu.UserId.Value, ct);

            var dtos = new List<ConversationDto>();
            foreach (var c in items)
            {
                unreadCounts.TryGetValue(c.Id, out var unread);
                var lastMsg = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
                dtos.Add(new ConversationDto
                {
                    Id = c.Id,
                    BuyerId = c.BuyerId,
                    BuyerName = c.Buyer?.FullName ?? "",
                    BuyerEmail = c.Buyer?.Email,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company?.CompanyName ?? "",
                    CompanyLogoUrl = c.Company?.LogoUrl,
                    Subject = c.Subject,
                    UnreadCount = unread,
                    LastMessage = lastMsg == null ? null : MapMessagePreview(lastMsg),
                    LastMessageAt = c.LastMessageAt,
                    CreatedAt = c.CreatedAt,
                    RelatedProductId = c.RelatedProductId,
                    RelatedOrderId = c.RelatedOrderId,
                    RelatedRfqId = c.RelatedRfqId
                });
            }

            return ApiResponse<PaginatedResult<ConversationDto>>.Ok(
                PaginatedResult<ConversationDto>.Success(dtos, total, req.Page, req.PageSize));
        }

        private static MessageDto MapMessagePreview(Message m) => new()
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
            CreatedAt = m.CreatedAt
        };
    }
}
