using System.Net.Http;
using System.Threading.Tasks;

namespace P1Simulator.Networking
{
    public class P1HttpClient
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _url;

        public P1HttpClient(string url)
        {
            _url = url;
        }

        public async Task<string> FetchTelegramAsync()
        {
            return await _httpClient.GetStringAsync(_url);
        }
    }
}
