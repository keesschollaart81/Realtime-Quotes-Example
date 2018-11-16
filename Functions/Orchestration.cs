using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName(nameof(Orchestration))]
        public static async Task<double> Orchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            [Queue("aggregations")] ICollector<AggregateAndPublishQuotesParams> queueCollector,
            ILogger logger,
            ExecutionContext executionContext)
        {
            var started = context.CurrentUtcDateTime;
            var orchestrationParams = context.GetInput<OrchestrationParams>();

            logger.LogMetric("OrchestratorStartLatency", (started - orchestrationParams.Started).TotalMilliseconds);

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

                try
                {
                    var quoteForSupplier = await firstFinishedTask;

                    quotesForSuppliers.Add(quoteForSupplier);

                    if (!context.IsReplaying) // only publish when we have new results incoming, this is particularly relevant when extended sessions is off
                    {
                        var aggregateAndPublishQuotesParams = new AggregateAndPublishQuotesParams(quotesForSuppliers);

                        // using a queue here, not using activity or sub orchestrator because we want fire&forget
                        queueCollector.Add(aggregateAndPublishQuotesParams);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Quote failed");

                    // do not fail orchestrator when broker request failed
                }
            }

            logger.LogMetric("OrchestratorDuration", (DateTime.UtcNow - started).TotalMilliseconds);

            var slowestBroker = quotesForSuppliers.Max(x => x.ResponseTime);

            // netto means than we dont include the wait time for the external broker
            var orchestratorDurationNetto = (DateTime.UtcNow - started).TotalMilliseconds - slowestBroker;
            logger.LogMetric("OrchestratorDurationNetto", orchestratorDurationNetto);

            return orchestratorDurationNetto;
        }
    }
}
