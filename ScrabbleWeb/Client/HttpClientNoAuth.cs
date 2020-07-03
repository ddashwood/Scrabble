using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client
{
    public class HttpClientNoAuth
    {
        public HttpClient Client { get; }

        public HttpClientNoAuth(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string url)
        {
            return await Client.GetFromJsonAsync<T>(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await Client.GetStringAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string url, TValue value)
        {
            return await Client.PostAsJsonAsync(url, value);
        }
    }
}
