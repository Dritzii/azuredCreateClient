using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azuredCreateClient
{
    class MicrosoftGraph
    {
        private static readonly HttpClient httpClient = new HttpClient();
        readonly string Token;

        public MicrosoftGraph(string Token)
        {
            this.Token = Token;
        }

        public async Task<string> GetMe()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.Token);
            string requestUrl = "https://graph.microsoft.com/v1.0/me";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);


            HttpResponseMessage response = await client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            return responseObject;
        }
    }
}
