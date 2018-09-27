using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName(nameof(GetSuppliersForCity))]
        public static string[] GetSuppliersForCity(
          [ActivityTrigger] DurableActivityContext activityContext,
          ILogger logger)
        {
            var city = activityContext.GetInput<string>();
            var Suppliers = city.ToArray().Select(c => new string(Enumerable.Repeat(c, 8).ToArray()));

            return Suppliers.ToArray();
        }
    }
}
