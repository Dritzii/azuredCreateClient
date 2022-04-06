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
        ManagementLogin managementlogin;
        string baseurl = "https://management.azure.com/subscriptions/";
        string token;

        public AzureServicesController(ManagementLogin managementlogin, string baseurl, string endpoint, string token)
        {
            this.managementlogin = managementlogin;
            this.baseurl = baseurl;
            this.token = token;
        }

        public async Task<List<string>> GetResourceByTag(string subscriptionId, string tag = "FWaaSAzured", string tagValue = "GatewaySubnetRoute", string resourceType = "Microsoft.Network/routeTables")
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            //GET https://management.azure.com/subscriptions/{subscriptionId}/resources?$filter={$filter}&$expand={$expand}&$top={$top}&api-version=2021-04-01
            string sendUrl = this.baseurl + subscriptionId + String.Format("/resources?$filter=resourceType eq {0}&api-version=2021-04-01", resourceType);
            Console.WriteLine(sendUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            //var jo = JObject.Parse(responseContent);
            //var access = jo["access_token"].ToString();
            //var refresh = jo["refresh_token"].ToString();
            //retList.AddRange(new List<string>() {
            //        access, refresh
            //    });
            //return retList;

        }
    }
}
