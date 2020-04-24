using Polynavi.Common.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Bll.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient httpClient;

        public HttpClientService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetResponseAsStringAsync(string uri, CancellationToken cts)
        {
            try // Move try catch?
            {
                using (var response = await httpClient.GetAsync(uri, cts))
                {
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException)
            {
                //TODO Log, throw?
                return null;
            }
        }
    }
}
