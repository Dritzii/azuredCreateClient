using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            string requestUrl = "https://graph.microsoft.com/v1.0/me";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;
        }

        public async Task<string> GetObjectId(string tenantId ,string clientId)
        {

            // https://stackoverflow.com/questions/49192583/azure-ad-returns-authentication-expiredtoken-on-valid-access-token
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            // https://graph.windows.net/tenant-id/servicePrincipalsByAppId/1c9fdc91-aaaa-aaaa-af8d-027507190f41/objectId?api-version=1.6
            string requestUrl = "https://graph.microsoft.com/v1.0/serviceprincipals?$filter=appId eq '" + clientId + "'";
            // string requestUrl = "https://graph.windows.net/" + tenantId + "/servicePrincipalsByAppId/" + clientId + "/objectId?api-version=1.6";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseContent);
            string subid = jo["value"][0]["id"].ToString();
            string subdisplayName = jo["value"][0]["displayName"].ToString();
            Console.WriteLine(subid);
            return subid;
        }
    }
}
