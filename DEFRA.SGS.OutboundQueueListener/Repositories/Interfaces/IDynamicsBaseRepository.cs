using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEFRA.SGS.Listener.Repositories.Interfaces;

public interface IDynamicsBaseRepository
{
    Task<HttpClient>  GetConnection(ILogger log);
    Task<dynamic> Get(HttpClient client, string query, ILogger log);
    Task<dynamic> Post(HttpClient client, string query, string content, ILogger log);
    Task<dynamic> Patch(HttpClient client, string query, string content, ILogger log);
}