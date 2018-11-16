
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName(nameof(ExecuteCitySearch))]
        public static async Task<IActionResult> ExecuteCitySearch(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
            ILogger log)
        {
            var city = req.Query["city"].FirstOrDefault();
            if (city == null) city = Guid.NewGuid().ToString().Substring(0, new Random().Next(2, 6));

            string taskId = string.Empty;

            if (city != null)
            {
                taskId = await orchestrationClient.StartNewAsync(nameof(Orchestration), new OrchestrationParams(city, DateTime.UtcNow));
            }

            return new OkObjectResult(new { city, taskId });
        }

        [FunctionName(nameof(CitySearchStatus))]
        public static async Task<IActionResult> CitySearchStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
            ILogger log)
        {
            var taskId = req.Query["taskId"].FirstOrDefault();

            var status = await orchestrationClient.GetStatusAsync(taskId);

            return new OkObjectResult(status);
        }

        [FunctionName("loaderio-verify")]
        public static IActionResult LoaderioVerify(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "loaderio-4d2aee32d783c95edf731a75b7f3ea88")]HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("loaderio-4d2aee32d783c95edf731a75b7f3ea88");
        }
    }
}
