using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName("GetBrokersForCity")]
        public static string[] GetBrokersForCity(
          [ActivityTrigger] DurableActivityContext activityContext,
          ILogger logger)
        {
            var city = activityContext.GetInput<string>();
            var brokers = city.ToArray().Select(c => new string(Enumerable.Repeat(c, 8).ToArray()));

            return brokers.ToArray();
        }
    }
}
