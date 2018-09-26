using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend
{
    public class QuotesHub : Hub
    {
        private readonly string FunctionsBaseUrl;

        public QuotesHub(IOptions<Appsettings> appsettingsOptions)
        {
            FunctionsBaseUrl = appsettingsOptions.Value.FunctionsBaseUrl;
        }

        public async Task GetQuotesRequest(string city)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"{FunctionsBaseUrl}/api/ExecuteCitySearch?city={city}");
                string resultContent = await result.Content.ReadAsStringAsync();
                await Clients.Client(Context.ConnectionId).SendAsync("GetQuotesRequestConfirmed", resultContent);
            }
        }
    }
}
