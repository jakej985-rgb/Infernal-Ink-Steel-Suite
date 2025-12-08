using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Domain
{
    public interface IQuoteRepository
    {
        bool AddQuote(Quote quote);
        List<Quote> GetAllQuotes();
    }
}
