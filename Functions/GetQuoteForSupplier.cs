using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName(nameof(GetQuoteForSupplier))]
        public static async Task<GetQuoteForSupplierResult> GetQuoteForSupplier(
          [ActivityTrigger] DurableActivityContext activityContext,
          ILogger logger)
        {
            var input = activityContext.GetInput<GetQuoteForSupplierParams>();

            //var rootUrl = Environment.GetEnvironmentVariable("RandomDataServiceRootUrl", EnvironmentVariableTarget.Process);
            var randomInt = new Random().Next(1, 8);

            var sw = new Stopwatch();
            sw.Start();
            //var response = await httpClient.GetAsync($"{rootUrl}/api/values/{input.City}/{input.TaskId}");
            var response = await httpClient.GetAsync($"https://realtimechannel.blob.core.windows.net/randomfiles/{randomInt}.json");
            response.EnsureSuccessStatusCode(); 

            var responseString = await response.Content.ReadAsStringAsync();
            var deserializedQuote = JsonConvert.DeserializeObject<RandomQuoteResult>(responseString);
            sw.Stop();

            return new GetQuoteForSupplierResult(input.Supplier, input.City, input.TaskId, deserializedQuote.Quote, sw.ElapsedMilliseconds);
        }

        private class RandomQuoteResult
        {
            public string City { get; set; }

            public double Quote { get; set; }

            public List<string> Data { get; set; }

        }
    }
}
