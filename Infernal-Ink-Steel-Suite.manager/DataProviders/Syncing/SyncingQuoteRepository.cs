using System.Collections.Generic;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingQuoteRepository : IQuoteRepository
    {
        private readonly IQuoteRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingQuoteRepository(IQuoteRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public bool AddQuote(Quote quote)
        {
            bool result = _inner.AddQuote(quote);
            if (result)
            {
                _syncQueue.Enqueue(new SyncQueueItem
                {
                    EntityType = "Quote",
                    EntityId = quote.Id,
                    Action = "Create",
                    PayloadJson = JsonSerializer.Serialize(quote)
                });
            }
            return result;
        }

        public List<Quote> GetAllQuotes() => _inner.GetAllQuotes();
    }
}
