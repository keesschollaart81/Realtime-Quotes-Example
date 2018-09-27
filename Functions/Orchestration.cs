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

        [FunctionName(nameof(Orchestration))]
        public static async Task Orchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger logger,
            ExecutionContext executionContext)
        {
            var started = context.CurrentUtcDateTime;
            var orchestrationParams = context.GetInput<OrchestrationParams>();

            var parallelTasks = new List<Task<GetQuoteForSupplierResult>>();

            var Suppliers = await context.CallActivityAsync<string[]>(nameof(GetSuppliersForCity), orchestrationParams.City);
            foreach (var Supplier in Suppliers)
            {
                var task = context.CallActivityAsync<GetQuoteForSupplierResult>(nameof(GetQuoteForSupplier), new GetQuoteForSupplierParams(Supplier, orchestrationParams.City, context.InstanceId));
                parallelTasks.Add(task);
            }

            var quotesForSuppliers = new List<GetQuoteForSupplierResult>();

            while (parallelTasks.Count > 0)
            {
                var firstFinishedTask = await Task.WhenAny(parallelTasks);
                parallelTasks.Remove(firstFinishedTask);

                var quoteForSupplier = await firstFinishedTask;

                quotesForSuppliers.Add(quoteForSupplier);

                await context.CallActivityAsync(nameof(AggregateAndPublishQuotes), new AggregateAndPublishQuotesParams(quotesForSuppliers));
            }
            
            telemetry.TrackMetric("OrchestratorStartLatency", (started - orchestrationParams.Started).TotalSeconds);
            telemetry.TrackMetric("OrchestratorDuration", (DateTime.UtcNow - started).TotalMilliseconds);
        }
    }
}
