
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PolyNaviLib.SL
{
    public static class HttpClientSL
    {
        public static async Task<string> GetResponseAsync(HttpClient client, string uri)
        {
            try
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
