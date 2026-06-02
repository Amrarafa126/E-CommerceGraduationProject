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
    public class ConversationRepos(AppDBContext Context) : GenericRepositoryAsync<Conversation>(Context), IConversationRepos
    {
        public async Task<Conversation?> GetWithMessagesAsync(
            Guid conversationId, int page, int pageSize, CancellationToken ct = default)
        {
            var conversation = await Context.conversations
                .Include(c => c.Buyer)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == conversationId, ct);

            if (conversation == null) return null;

            var messages = await Context.messages
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .Include(m => m.ReadReceipts).ThenInclude(r => r.User)
                .Include(m => m.ReplyToMessage).ThenInclude(rm => rm!.Sender)
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending (m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            foreach (var msg in messages)
                conversation.Messages.Add(msg);

            return conversation;
        }

        public async Task<Conversation?> FindBetweenAsync(
            Guid buyerId, Guid companyId, CancellationToken ct = default)
            => await Context.conversations
                .FirstOrDefaultAsync(c =>
                    c.BuyerId == buyerId && c.CompanyId == companyId, ct);

        public async Task<(IEnumerable<Conversation> Items, int Total)> GetByBuyerAsync(
            Guid buyerId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = Context.conversations
                .AsNoTracking()
                .Include(c => c.Buyer)
                .Include(c => c.Company)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(c => c.BuyerId == buyerId && !c.IsBuyerArchived);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(c => c.LastMessageAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<(IEnumerable<Conversation> Items, int Total)> GetByCompanyAsync(
            Guid companyId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = Context.conversations
                .AsNoTracking()
                .Include(c => c.Buyer)
                .Include(c => c.Company)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(c => c.CompanyId == companyId && !c.IsCompanyArchived);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(c => c.LastMessageAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<int> CountUnreadAsync(
            Guid conversationId, Guid receiverId, CancellationToken ct = default)
            => await Context.messages
                .CountAsync(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != receiverId &&
                    !m.IsRead, ct);

        public async Task<Dictionary<Guid, int>> CountUnreadBatchAsync(
            IEnumerable<Guid> conversationIds, Guid receiverId, CancellationToken ct = default)
        {
            var ids = conversationIds.ToList();
            if (!ids.Any()) return new Dictionary<Guid, int>();

            return await Context.messages
                .Where(m => ids.Contains(m.ConversationId) && m.SenderId != receiverId && !m.IsRead)
                .GroupBy(m => m.ConversationId)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);
        }
    }
}

