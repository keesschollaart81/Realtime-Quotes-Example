using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName("AggregateAndPublishQuotes")]
        public static async Task AggregateAndPublishQuotes(
          [ActivityTrigger] DurableActivityContext activityContext,
          [SignalR(HubName = "quoteshub")]IAsyncCollector<SignalRMessage> signalRMessages,
          ILogger logger)
        {
            var input = activityContext.GetInput<AggregateAndPublishQuotesParams>();

            var updatedQuotes = input.QuoteResponses.OrderBy(x => x.Quote);
            foreach (var updatedQuote in updatedQuotes)
            {
                updatedQuote.Quote = Math.Ceiling(updatedQuote.Quote);
            }

            logger.LogInformation($"Publishing {updatedQuotes.Count()} quotes");

            await signalRMessages.AddAsync(new SignalRMessage
            {
                //UserId = input.CorrelationId,
                Target = "quotePosted",
                Arguments = new[]{ updatedQuotes.Select(x => new
                {
                    x.Quote,
                    x.City,
                    x.Broker,
                    x.TaskId,
                    x.ResponseTime
                }).ToArray()}
            });
        }
    }
}
