using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class SubscriptionsController
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://management.azure.com/subscriptions?api-version=2020-01-01";
        readonly string Token;
        public JsonSerializerSettings jss = new JsonSerializerSettings();

        public SubscriptionsController(string Token)
        {
            this.Token = Token;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public async Task<List<string>> GetAllSubscriptionsAsync()
        {
            var retList = new List<string>();
            Console.WriteLine(hostUrl);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrl);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseContent);
            foreach (var items in jo["value"])
            {
                var c1 = items["displayName"].Value<string>();
                var c2 = items["subscriptionId"].Value<string>();
                retList.AddRange(new List<string>() { 
                    c1, c2 
                });
            }
            return retList;
        }

        public static int filterResourceByName(List<string> listName)
        {
            int index = listName.FindIndex(x => x.Contains("CSP"));
            return index - 1;
        }

        public async Task<string> GetAllSubscriptionsAsyncJO()
        {
            var retList = new List<string>();
            Console.WriteLine(hostUrl);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrl);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseContent);
            return (string)jo;
        }


        public async Task<string> GetAllSubscriptionsGraph()
        {
            Console.WriteLine(hostUrl);
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
