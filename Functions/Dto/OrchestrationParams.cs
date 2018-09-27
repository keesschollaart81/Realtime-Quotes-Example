using System;

namespace RealtimeQuotes.Functions
{
    public class OrchestrationParams
    {
        public OrchestrationParams(string city, DateTime started)
        {
            City = city;
            Started = started;
        }

        public string City { get; set; }

        public DateTime Started { get; }
    }
}
