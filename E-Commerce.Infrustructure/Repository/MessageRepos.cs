using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.InfrustructureBases;
using E_Commerce.Infrustructure.Interfase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Repository
{
    public class MessageRepos(AppDBContext Context) : GenericRepositoryAsync<Message>(Context), IMessageRepos
    {
        public async Task MarkAllReadAsync(
       Guid conversationId, Guid receiverId, CancellationToken ct = default)
        {
            var unread = await Context.messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != receiverId &&
                    !m.IsRead)
                .ToListAsync(ct);

            foreach (var msg in unread)
                msg.MarkAsRead();

            await Context.SaveChangesAsync(ct);
        }
    }
}
