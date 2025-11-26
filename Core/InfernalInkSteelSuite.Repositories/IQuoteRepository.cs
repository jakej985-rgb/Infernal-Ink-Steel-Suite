using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IQuoteRepository
    {
        bool AddQuote(Quote quote);
        List<Quote> GetAllQuotes();
    }
}
