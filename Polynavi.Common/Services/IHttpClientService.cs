using System.Threading;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface IHttpClientService
    {
        Task<string> GetResponseAsStringAsync(string uri, CancellationToken cts);
    }
}
