
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName("ExecuteCitySearch")]
        public static async Task<IActionResult> ExecuteCitySearch(
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
            ILogger log)
        {
            var city = req.Query["city"].FirstOrDefault();

            string taskId = string.Empty;

            if (city != null)
            {
                taskId = await orchestrationClient.StartNewAsync("Orchestration", new OrchestrationParams(city));
            }

            return new OkObjectResult(taskId);
        }
    }
}
