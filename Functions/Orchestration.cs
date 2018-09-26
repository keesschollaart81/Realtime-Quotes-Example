using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName("Orchestration")]
        public static async Task Orchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger logger,
            ExecutionContext executionContext)
        {
            var orchestrationParams = context.GetInput<OrchestrationParams>();

            var parallelTasks = new List<Task<GetQuoteForBrokerResponse>>();

            var brokers = await context.CallActivityAsync<string[]>("GetBrokersForCity", orchestrationParams.City);
            foreach (var broker in brokers)
            {
                var task = context.CallActivityAsync<GetQuoteForBrokerResponse>("GetQuoteForBroker", new GetQuoteForBrokerParams(broker, orchestrationParams.City, context.InstanceId));
                parallelTasks.Add(task);
            }

            var getQuoteForBrokerResponses = new List<GetQuoteForBrokerResponse>();

            while (parallelTasks.Count > 0)
            {
                var firstFinishedTask = await Task.WhenAny(parallelTasks);
                parallelTasks.Remove(firstFinishedTask);

                var getQuoteForBrokerResponse = await firstFinishedTask;

                getQuoteForBrokerResponses.Add(getQuoteForBrokerResponse);

                await context.CallActivityAsync("AggregateAndPublishQuotes", new AggregateAndPublishQuotesParams(getQuoteForBrokerResponses));
            }
        }
    }
}
