using System.Collections.Generic;

namespace RealtimeQuotes.Functions
{
    public class AggregateAndPublishQuotesParams
    {
        public AggregateAndPublishQuotesParams(List<GetQuoteForBrokerResponse> quoteResponses)
        {
            QuoteResponses = quoteResponses;
        }

        public List<GetQuoteForBrokerResponse> QuoteResponses { get; }
    }
    public class GetQuoteForBrokerParams
    {
        public GetQuoteForBrokerParams(string broker, string city, string taskId)
        {
            Broker = broker;
            City = city;
            TaskId = taskId;
        }

        public string City { get; set; }

        public string Broker { get; set; }

        public string TaskId { get; set; }
    }
    public class GetQuoteForBrokerResponse
    {
        public GetQuoteForBrokerResponse(string broker, string city, string taskId, double quote, double responseTime)
        {
            Broker = broker;
            City = city;
            TaskId = taskId;
            Quote = quote;
            ResponseTime = responseTime;
        }

        public string City { get; set; }

        public string Broker { get; set; }

        public string TaskId { get; set; }

        public double Quote { get; set; }

        public double ResponseTime { get; }
    }
}
