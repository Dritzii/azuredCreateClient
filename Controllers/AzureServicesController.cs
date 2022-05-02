using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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


        public async Task<List<string>> GetResourceByTag(string subscriptionId, string resourceType = "Microsoft.Network/routeTables")
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //GET https://management.azure.com/subscriptions/{subscriptionId}/resources?$filter={$filter}&$expand={$expand}&$top={$top}&api-version=2021-04-01
            string sendUrl = baseurl + subscriptionId + String.Format("/resources?$filter=resourceType eq '{0}'&api-version=2021-04-01", resourceType);
            //Console.WriteLine(sendUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            foreach (var items in jo["value"])
            {
                var c1 = items["id"].Value<string>();
                //var c2 = items["name"].Value<string>();
                //var c3 = items["type"].Value<string>();
                //var c4 = items["location"].Value<string>();
                var c5 = items?["tags"].ToString();
                retList.AddRange(new List<string>() {
                    c1, c5
                });
            }
            return retList;
        }

        public async Task<List<string>> GetResourceGroupById(string id)
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //GET https://management.azure.com/{resourceId}?api-version=2021-04-01
            string sendUrl = baseurl + id + "?api-version=2021-04-01";
            //Console.WriteLine(sendUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            var access = jo["access_token"].ToString();
            var refresh = jo["refresh_token"].ToString();
            retList.AddRange(new List<string>() {
                    access, refresh
                });
            return retList;

        }

        public async void NewGatewayRoute(string id, string ipaddress, string profixName = "NMAgent-", string prefix = "/32", string nexthoptype = "internet")
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            char[] charsToTrimStart = { '/','s', 'u', 'b', 's', 'c', 'r', 'i', 'p', 't', 'i', 'o', 'n', 's', '/' };
            string idTrimmed = id.TrimStart(charsToTrimStart);
            string sendUrl = baseurl + idTrimmed + string.Format("/routes/{0}?api-version=2021-04-01", "NMAgent-" + ipaddress);
            var payload = new { name = profixName + ipaddress , properties = new { addressPrefix = ipaddress + prefix, nextHopType = nexthoptype } };
            var jsonToReturn = JsonConvert.SerializeObject(payload);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, sendUrl) {
                Content = new StringContent(jsonToReturn.ToString(), Encoding.UTF8, "application/json"),
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
        public async Task<string> GetRouteTable(string id)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            char[] charsToTrimStart = { '/', 's', 'u', 'b', 's', 'c', 'r', 'i', 'p', 't', 'i', 'o', 'n', 's', '/' };
            string idTrimmed = id.TrimStart(charsToTrimStart);
            string sendUrl = baseurl + idTrimmed + "?api-version=2021-05-01";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl){};
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        public async void updateOrCreateRouteTableWithRoutes(string id, string routes, string location = "australiasoutheast")
        {
            //Console.WriteLine(routes.ToString());
            using var client = new HttpClient();
            //JObject jo = JObject.Parse(routes);
            //JArray a = JArray.Parse(routes);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //string routeTrimmed = routes.Replace(@"\r\n", "");
            string[] charactersToReplace = new string[] { @"\t", @"\n", @"\r", " " , @"\r\n", @"\" };
            foreach (string s in charactersToReplace)
            {
                routes = routes.Replace(s, "");
            }
            // https://management.azure.com/subscriptions/6c737636-bd1d-49fd-8eea-48d69ae27155/resourceGroups/rg_NetworkConfigurations/providers/Microsoft.Network/routeTables/johnAzuredTest?api-version=2021-04-01
            char[] charsToTrimStart = { '/', 's', 'u', 'b', 's', 'c', 'r', 'i', 'p', 't', 'i', 'o', 'n', 's', '/' };
            string idTrimmed = id.TrimStart(charsToTrimStart);
            string sendUrl = baseurl + idTrimmed + "?api-version=2021-04-01";
            var payload = new { properties = new { routes = routes.ToString() }, location = location , tags = new { FWaaSAzured = "GatewaySubnetRoute"}};
            //Console.WriteLine(payload.ToString());
            //JObject o = new JObject{
            //    "properties",
            //    {
            //    "routes", new JArray { routes.ToString() }
            //    },
            //    "tags",{"FWaaSAzured", "GatewaySubnetRoute"},
            //   "location", "australiasoutheast"
            
            //};
            //Console.WriteLine(a.ToString());
            //Console.WriteLine(o.ToString());
            var jsonToReturn = JsonConvert.SerializeObject(payload);
            //Console.WriteLine(jsonToReturn.ToString());
            /*
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, sendUrl)
            {
                Content = new StringContent(jsonToReturn.ToString(), Encoding.UTF8, "application/json"),
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            */

        }
    }
}
