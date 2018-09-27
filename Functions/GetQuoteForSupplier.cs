﻿using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RealtimeQuotes.Functions
{
    public static partial class ExecuteCitySearchFunctions
    {
        [FunctionName(nameof(GetQuoteForSupplier))]
        public static async Task<GetQuoteForSupplierResult> GetQuoteForSupplier(
          [ActivityTrigger] DurableActivityContext activityContext,
          ILogger logger)
        {
            var input = activityContext.GetInput<GetQuoteForSupplierParams>();

            var quote = new Random().NextDouble() * 10; // random quote between 0 and 10

            var responseTime = TimeSpan.FromSeconds(new Random().Next(10));
            // simulate fast and slow HTTP request to a Supplier
            //await Task.Delay(responseTime);

            return new GetQuoteForSupplierResult(input.Supplier, input.City, input.TaskId, quote, responseTime.TotalSeconds);
        }
    }
}