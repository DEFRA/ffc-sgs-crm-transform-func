using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DEFRA.SGS.Listener.Extensions;
using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DEFRA.SGS.Listener.Repositories
{
    public class DynamicsBaseRepository : IDynamicsBaseRepository
    {
        protected HttpClient _httpClient;
        private Config _config;
        private static AuthenticationResult _authenticationResult;

        public DynamicsBaseRepository(HttpClient httpClient, Config config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<HttpClient>  GetConnection(ILogger log = null)
        {
            if (!_config.IsLocalTest)
            {
                if (_authenticationResult == null || DateTime.UtcNow.AddMinutes(1) > _authenticationResult.ExpiresOn)
                {
                    var clientCrendential = new ClientCredential(_config.ClientId, _config.Secret);
                    var authContext = new AuthenticationContext(_config.Authority);
                    var authResult = authContext.AcquireTokenAsync(_config.BaseAddress, clientCrendential).Result;

                    if (authResult == null)
                    {
                        log?.LogInformation($"Cannot get token.");
                        throw new Exception("Cannot acquire login token");
                    }

                    _authenticationResult = authResult;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationResult.AccessToken);
            }
            return _httpClient;
        }

        public async Task<dynamic> Get(HttpClient client, string query, ILogger log)
        {
            try
            {
                var response = client.GetAsync(query).Result;

                if (response.IsSuccessStatusCode)
                {
                    var records = response.Content.ReadAsStringAsync().Result;
                    var jRetrieveResponse = JObject.Parse(records);

                    dynamic collection = JsonConvert.DeserializeObject(jRetrieveResponse.ToString());

                    return collection;
                }
                else
                {
                    log?.LogError("GET operation failed: {0}", response.ReasonPhrase);
                    throw ParseError(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> Post(HttpClient client, string query, string content, ILogger log)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, query)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                using (HttpResponseMessage response = await SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        log?.LogInformation("POST operation succeeded: {0}", response.ReasonPhrase);
                        var record = response.Content.ReadAsStringAsync().Result;

                        var jRetrieveResponse = JObject.Parse(record);

                        dynamic postResponseObject = JsonConvert.DeserializeObject(jRetrieveResponse.ToString());

                        return postResponseObject;
                    }
                    else
                    {
                        log?.LogError("POST operation failed: {0}", response.ReasonPhrase);
                        throw ParseError(response);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> Patch(HttpClient client, string query, string content, ILogger log)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, query)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                using (HttpResponseMessage response = await SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        log?.LogInformation("PATCH operation succeeded: {0}", response.ReasonPhrase);
                        var record = response.Content.ReadAsStringAsync().Result;

                        var jRetrieveResponse = JObject.Parse(record);

                        dynamic postResponseObject = JsonConvert.DeserializeObject(jRetrieveResponse.ToString());

                        return postResponseObject;
                    }
                    else
                    {
                        log?.LogError("PATCH operation failed: {0}", response.ReasonPhrase);
                        throw ParseError(response);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        protected ServiceException ParseError(HttpResponseMessage response)
        {
            try
            {
                int code = 0;
                string message = "no content returned",
                       content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (content.Length > 0)
                {
                    var errorObject = JObject.Parse(content);
                    message = errorObject["error"]["message"].Value<string>();
                    //code = Convert.ToInt32(errorObject["error"]["code"].Value<string>(), 16);
                }
                int statusCode = (int)response.StatusCode;
                string reasonPhrase = response.ReasonPhrase;

                return new ServiceException(code, statusCode, reasonPhrase, message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends all requests with retry capabilities
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="httpCompletionOption">Indicates if HttpClient operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content.</param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <returns>The response for the request.</returns>
        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead,
            int retryCount = 0)
        {
            HttpResponseMessage response;
            try
            {
                //The request is cloned so it can be sent again.
                response = await _httpClient.SendAsync(request.Clone(), httpCompletionOption);
            }
            catch (Exception)
            {
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode != 429)
                {
                    //Not a service protection limit error
                    throw ParseError(response);
                }
                else
                {
                    // Give up re-trying if exceeding the maxRetries
                    if (++retryCount >= _config.MaxRetries)
                    {
                        throw ParseError(response);
                    }

                    int seconds;
                    //Try to use the Retry-After header value if it is returned.
                    if (response.Headers.Contains("Retry-After"))
                    {
                        seconds = int.Parse(response.Headers.GetValues("Retry-After").FirstOrDefault());
                    }
                    else
                    {
                        //Otherwise, use an exponential backoff strategy
                        seconds = (int)Math.Pow(2, retryCount);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));

                    return await SendAsync(request, httpCompletionOption, retryCount);
                }
            }
            else
            {
                return response;
            }
        }
    }
}
