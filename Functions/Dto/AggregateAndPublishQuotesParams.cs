using System.Collections.Generic;

namespace RealtimeQuotes.Functions
{
    public class AggregateAndPublishQuotesParams
    {
        public AggregateAndPublishQuotesParams(List<GetQuoteForSupplierResult> quoteResponses)
        {
            QuoteResponses = quoteResponses;
        }

        public List<GetQuoteForSupplierResult> QuoteResponses { get; }
    }
}
