using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Oueries.Models;
using E_Commerce.Core.Wrappers;
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

namespace E_Commerce.Core.Features.Chats.Oueries.Handlers
{
    public class ChatHandlerQueries(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetConversationsQuery, ApiResponse<PaginatedResult<ConversationDto>>>,
        IRequestHandler<GetConversationMessagesQuery, ApiResponse<ConversationPageDto>>
    {
        public async Task<ApiResponse<PaginatedResult<ConversationDto>>> Handle(
        GetConversationsQuery req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var user = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(User), currentUser.UserId.Value);

            IEnumerable<Conversation> items;
            int total;

            if (user.CompanyId != null)
            {
                (items, total) = await uow.Conversations.GetByCompanyAsync(
                    user.CompanyId, req.Page, req.PageSize, ct);
            }
            else
            {
                (items, total) = await uow.Conversations.GetByBuyerAsync(
                    currentUser.UserId.Value, req.Page, req.PageSize, ct);
            }

            var dtos = items.Select(c => new ConversationDto(
                c.Id,
                c.BuyerId,
                c.Buyer?.FullName ?? "",
                c.Buyer?.Email,
                c.CompanyId,
                c.Company?.CompanyName ?? "",
                c.Company?.LogoUrl,
                0, // unread count – loaded separately for performance
                c.Messages.OrderByDescending(m => m.CreatedAt).Select(m => new MessageDto(
                    m.Id, m.ConversationId, m.SenderId,
                    m.Sender?.FullName ?? "", m.Content, m.AttachmentUrl,
                    m.Type.ToString(), m.IsRead, m.ReadAt, m.CreatedAt)).FirstOrDefault(),
                c.LastMessageAt,
                c.CreatedAt)).ToList();

            return ApiResponse<PaginatedResult<ConversationDto>>.Ok(
                PaginatedResult<ConversationDto>.Success(dtos, total, req.Page, req.PageSize));
        }

        public async Task<ApiResponse<ConversationPageDto>> Handle(
       GetConversationMessagesQuery req, CancellationToken ct)
        {
            if (currentUser.UserId == null) throw new UnauthorizedException();

            var conversation = await uow.Conversations.GetWithMessagesAsync(
                req.ConversationId, req.Page, req.PageSize, ct)
                ?? throw new NotFoundException(nameof(Conversation), req.ConversationId);

            var user = await uow.Users.GetByIdAsync(currentUser.UserId.Value, ct)!;
            bool isBuyer = conversation.BuyerId == currentUser.UserId.Value;
            bool isCompanyMember = user?.CompanyId == conversation.CompanyId;

            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("Access denied to this conversation.");

            var messageDtos = conversation.Messages
                .Select(m => new MessageDto(
                    m.Id, m.ConversationId, m.SenderId,
                    m.Sender?.FullName ?? "",
                    m.Content, m.AttachmentUrl,
                    m.Type.ToString(), m.IsRead, m.ReadAt, m.CreatedAt))
                .ToList();

            var convDto = new ConversationDto(
                conversation.Id,
                conversation.BuyerId,
                conversation.Buyer?.FullName ?? "",
                conversation.Buyer?.Email,
                conversation.CompanyId,
                conversation.Company?.CompanyName ?? "",
                conversation.Company?.LogoUrl,
                0,
                messageDtos.LastOrDefault(),
                conversation.LastMessageAt,
                conversation.CreatedAt);

            var totalMessages = await uow.Conversations.CountUnreadAsync(
                req.ConversationId, currentUser.UserId.Value, ct);

            return ApiResponse<ConversationPageDto>.Ok(
                new ConversationPageDto(convDto, messageDtos, totalMessages, req.Page, req.PageSize));
        }
    }
}
