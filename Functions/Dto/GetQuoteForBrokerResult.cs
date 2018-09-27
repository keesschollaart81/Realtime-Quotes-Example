namespace RealtimeQuotes.Functions
{

    public class GetQuoteForSupplierResult
    {
        public GetQuoteForSupplierResult(string supplier, string city, string taskId, double quote, double responseTime)
        {
            Supplier = supplier;
            City = city;
            TaskId = taskId;
            Quote = quote;
            ResponseTime = responseTime;
        }

        public string City { get; set; }

        public string Supplier { get; set; }

        public string TaskId { get; set; }

        public double Quote { get; set; }

        public double ResponseTime { get; }
    }
}
