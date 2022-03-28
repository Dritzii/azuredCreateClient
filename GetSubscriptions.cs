using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class GetSubscriptions
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://management.azure.com/subscriptions?api-version=2020-01-01";
        string Token;

        public GetSubscriptions(string Token)
        {
            this.Token = Token;
        }

        public async Task<string> GetAllSubscriptionsAsync()
        {
            Console.WriteLine(hostUrl);
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrl);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            string subid = jo["value"][0]["subscriptionId"].ToString();
            Console.WriteLine("subscription id " + subid);
            return subid;


        }
        public async Task<string> GetAllSubscriptionsGraph()
        {
            Console.WriteLine(hostUrl);
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            string hostUrltest = "https://graph.microsoft.com/v1.0/subscriptions";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrltest);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            string subid = jo["value"][0]["subscriptionId"].ToString();
            Console.WriteLine("subscription id " + subid);
            return subid;


        }

    }
}
