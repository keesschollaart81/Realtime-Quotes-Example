namespace RealtimeQuotes.Functions
{
    public class GetQuoteForSupplierParams
    {
        public GetQuoteForSupplierParams(string supplier, string city, string taskId)
        {
            Supplier = supplier;
            City = city;
            TaskId = taskId;
        }

        public string City { get; set; }

        public string Supplier { get; set; }

        public string TaskId { get; set; }
    }
}
