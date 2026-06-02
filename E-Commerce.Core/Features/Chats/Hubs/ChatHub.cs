using E_Commerce.Core.Exceptions;
using E_Commerce.Service.Interfase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce.Core.Features.Chats.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinConversation(Guid conversationId)
        {
            var groupName = $"conv_{conversationId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            var groupName = $"conv_{conversationId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task Typing(Guid conversationId, bool isTyping)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            var userName = Context.User?.FindFirst("given_name")?.Value
                ?? Context.User?.Identity?.Name
                ?? "User";

            if (string.IsNullOrEmpty(userId)) return;

            var groupName = $"conv_{conversationId}";
            await Clients.OthersInGroup(groupName).SendAsync("TypingIndicator", new
            {
                ConversationId = conversationId,
                UserId = userId,
                UserName = userName,
                IsTyping = isTyping
            });
        }
    }
}
