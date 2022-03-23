using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azuredCreateClient
{
    class GetSubscriptions
    {
        private static readonly string hostUrl = "https://management.azure.com/subscriptions?api-version=2020-01-01";
        string Token;

        public GetSubscriptions(string Token)
        {
            this.Token = Token;
        }

        public async Task<string> GetAllSubscriptionsAsync() 
        {

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            Console.WriteLine(this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrl);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;

        }
    }
}
