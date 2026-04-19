using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfernalInkSteelSuite.Repositories
{
    public class QuoteRepository(AppDbContext dbContext) : IQuoteRepository
    {
        private readonly AppDbContext _db = dbContext;

        public bool AddQuote(Quote quote)
        {
            quote.CreatedAt = DateTime.UtcNow;
            _db.Quotes.Add(quote);
            return _db.SaveChanges() > 0;
        }

        public List<Quote> GetAllQuotes()
        {
            return _db.Quotes.OrderByDescending(q => q.CreatedAt).ToList();
        }
    }
}
