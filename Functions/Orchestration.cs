using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        private static string key = TelemetryConfiguration.Active.InstrumentationKey = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private static TelemetryClient telemetry = new TelemetryClient() { InstrumentationKey = key };

        [FunctionName("Orchestration")]
        public static async Task Orchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger logger,
            ExecutionContext executionContext)
        {
            var started = context.CurrentUtcDateTime;
            var orchestrationParams = context.GetInput<OrchestrationParams>();

            var parallelTasks = new List<Task<GetQuoteForBrokerResponse>>();

            var brokers = await context.CallActivityAsync<string[]>(nameof(GetBrokersForCity), orchestrationParams.City);
            foreach (var broker in brokers)
            {
                var task = context.CallActivityAsync<GetQuoteForBrokerResponse>(nameof(GetQuoteForBroker), new GetQuoteForBrokerParams(broker, orchestrationParams.City, context.InstanceId));
                parallelTasks.Add(task);
            }

            var quotesForBrokers = new List<GetQuoteForBrokerResponse>();

            while (parallelTasks.Count > 0)
            {
                var firstFinishedTask = await Task.WhenAny(parallelTasks);
                parallelTasks.Remove(firstFinishedTask);

                var quoteForBroker = await firstFinishedTask;

                quotesForBrokers.Add(quoteForBroker);

                await context.CallActivityAsync(nameof(AggregateAndPublishQuotes), new AggregateAndPublishQuotesParams(quotesForBrokers));
            }
            
            telemetry.TrackMetric("OrchestratorStartLatency", (started - orchestrationParams.Started).TotalSeconds);
            telemetry.TrackMetric("OrchestratorDuration", (DateTime.UtcNow - started).TotalMilliseconds);
        }
    }
}
