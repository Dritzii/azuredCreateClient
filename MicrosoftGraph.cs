using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azuredCreateClient
{
    class MicrosoftGraph
    {
        readonly string Token;

        public MicrosoftGraph(string Token)
        {
            this.Token = Token;
        }

        public async Task<string> GetMe()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            string requestUrl = "https://graph.microsoft.com/v1.0/me";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);


            HttpResponseMessage response = await client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;
        }
    }
}
