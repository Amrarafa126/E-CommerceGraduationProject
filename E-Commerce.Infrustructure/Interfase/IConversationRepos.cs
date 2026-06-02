using E_Commerce.Data.Entity;
using E_Commerce.Infrustructure.InfrustructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Interfase
{
    public interface IConversationRepos : IGenericRepositoryAsync<Conversation>
    {
        Task<Conversation?> GetWithMessagesAsync(Guid conversationId, int page, int pageSize, CancellationToken ct = default);
        Task<Conversation?> FindBetweenAsync(Guid buyerId, Guid companyId, CancellationToken ct = default);
        Task<(IEnumerable<Conversation> Items, int Total)> GetByBuyerAsync(Guid buyerId, int page, int pageSize, CancellationToken ct = default);
        Task<(IEnumerable<Conversation> Items, int Total)> GetByCompanyAsync(Guid companyId, int page, int pageSize, CancellationToken ct = default);
        Task<int> CountUnreadAsync(Guid conversationId, Guid receiverId, CancellationToken ct = default);
        Task<Dictionary<Guid, int>> CountUnreadBatchAsync(IEnumerable<Guid> conversationIds, Guid receiverId, CancellationToken ct = default);
    }
}
