using AutoMapper;
using E_Commerce.Core.Exceptions;
using E_Commerce.Core.Features.Chats.Commands.Models;
using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.InterFaseUnitOfWork;
using E_Commerce.Core.Features.Chats.Hubs;
using E_Commerce.Service.Interfase;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce.Core.Features.Chats.Commands.Handlers
{
    public class MarkMessagesReadHandler(
        IUnitOfWork uow,
        ICurrentUserService cu,
        IHubContext<ChatHub> hubContext)
        : IRequestHandler<MarkMessagesReadCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(MarkMessagesReadCommand req, CancellationToken ct)
        {
            if (cu.UserId == null)
                throw new UnauthorizedException();

            var conversation = await uow.Conversations.GetByIdAsync(req.ConversationId, ct)
                ?? throw new NotFoundException(nameof(Conversation), req.ConversationId);

            bool isBuyer = conversation.BuyerId == cu.UserId.Value;
            bool isCompanyMember = cu.OwnedCompanyId == conversation.CompanyId;
            if (!isBuyer && !isCompanyMember)
                throw new ForbiddenException("أنت لست مشاركاً في هذه المحادثة.");

            var user = await uow.Users.GetByIdAsync(cu.UserId.Value, ct)
                ?? throw new NotFoundException(nameof(User), cu.UserId.Value);

            var messages = (await uow.Messages.FindAsync(m => req.MessageIds.Contains(m.Id), ct)).ToList();
            var readAt = DateTime.UtcNow;
            var readReceipts = new List<MessageReadReceipt>();

            foreach (var message in messages)
            {
                if (message.SenderId == cu.UserId.Value) continue; // Don't mark own messages

                bool alreadyRead = await uow.MessageReadReceipts.ExistsAsync(
                    r => r.MessageId == message.Id && r.UserId == cu.UserId.Value, ct);
                if (alreadyRead) continue;

                var receipt = MessageReadReceipt.Create(message.Id, cu.UserId.Value);
                readReceipts.Add(receipt);

                // Mark message as read so unread counts decrease
                if (!message.IsRead)
                {
                    message.MarkAsRead();
                    uow.Messages.Update(message);
                }
            }

            if (readReceipts.Count > 0)
            {
                foreach (var receipt in readReceipts)
                    await uow.MessageReadReceipts.AddAsync(receipt, ct);

                await uow.SaveChangesAsync(ct);

                // Broadcast read receipt via SignalR
                var groupName = $"conv_{conversation.Id}";
                await hubContext.Clients.Group(groupName)
                    .SendAsync("MessagesRead", new ReadReceiptBroadcastDto
                    {
                        ConversationId = conversation.Id,
                        MessageIds = readReceipts.Select(r => r.MessageId).ToList(),
                        ReaderId = cu.UserId.Value,
                        ReaderName = user.FullName,
                        ReadAt = readAt
                    }, ct);
            }

            return ApiResponse<object>.Ok("تم تحديد الرسائل كمقروءة.");
        }
    }
}
