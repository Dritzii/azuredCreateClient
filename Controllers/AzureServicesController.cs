using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace azuredCreateClient.Controllers
{
    class AzureServicesController
    {
        string baseurl = "https://management.azure.com/subscriptions/";
        string token;
        readonly JsonSerializerSettings jss = new JsonSerializerSettings();

        public AzureServicesController(string token)
        {
            this.token = token;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public async Task<List<string>> GetResourceByTag(string subscriptionId, string tag = "FWaaSAzured", string tagValue = "GatewaySubnetRoute", string resourceType = "Microsoft.Network/routeTables")
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //GET https://management.azure.com/subscriptions/{subscriptionId}/resources?$filter={$filter}&$expand={$expand}&$top={$top}&api-version=2021-04-01
            string sendUrl = baseurl + subscriptionId + String.Format("/resources?$filter=resourceType eq '{0}'&api-version=2021-04-01", resourceType);
            Console.WriteLine(sendUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            foreach (var items in jo["value"])
            {
                var c1 = items["id"].Value<string>();
                var c2 = items["name"].Value<string>();
                var c3 = items["type"].Value<string>();
                var c4 = items["location"].Value<string>();
                var c5 = items["tags"].Value<string>();
                //var c6 = items["tags"]["FWaaSAzured"].Value<string>();
                retList.AddRange(new List<string>() {
                    c1, c2, c3, c4, c5//, c6
                });
            }
            Console.WriteLine(retList);
            return retList;

        }

        public async Task<List<string>> GetResourceGroupById(string id)
        {
            // GET https://management.azure.com/{resourceId}?api-version=2021-04-01
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //GET https://management.azure.com/{resourceId}?api-version=2021-04-01
            string sendUrl = baseurl + id + "?api-version=2021-04-01";
            Console.WriteLine(sendUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            var access = jo["access_token"].ToString();
            var refresh = jo["refresh_token"].ToString();
            retList.AddRange(new List<string>() {
                    access, refresh
                });
            return retList;

        }
        
    }
}
