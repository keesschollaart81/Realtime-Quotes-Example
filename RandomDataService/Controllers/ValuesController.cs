using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("{input}/{taskId}")]
        public async Task<ActionResult<RandomQuoteResult>> Get(string input, string taskId)
        {
            var randomDelay = new Random().Next(0, 10);
            await Task.Delay(TimeSpan.FromSeconds(randomDelay));

            var quote = new Random().NextDouble() * 50; // random quote between 0 and 50

            var randomPayload = new List<string>();
            for (var i = 0; i < randomDelay; i++)
            {
                randomPayload.Add(new String('*', new Random().Next(1000, 483647)));
            }

            if (quote > 48) throw new Exception("Once in a while...");

            return new RandomQuoteResult(input, quote, randomPayload);
        }
    }
    public class RandomQuoteResult
    {
        public RandomQuoteResult(string city, double quote, List<string> data)
        {
            City = city;
            Quote = quote;
            Data = data;
        }

        public string City { get; set; }

        public double Quote { get; set; }

        public List<string> Data { get; set; }

    }
}
