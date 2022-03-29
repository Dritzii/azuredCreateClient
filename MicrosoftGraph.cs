using System;
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
        JsonSerializerSettings jss = new JsonSerializerSettings();
        

        public MicrosoftGraph(string Token)
        {
            this.Token = Token;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public async Task<string> GetMe()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            string requestUrl = "https://graph.microsoft.com/v1.0/me";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;
        }

        public async Task<string> GetObjectId(string clientId)
        {
            // https://stackoverflow.com/questions/49192583/azure-ad-returns-authentication-expiredtoken-on-valid-access-token
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            string requestUrl = "https://graph.microsoft.com/v1.0/serviceprincipals?$filter=appId eq '" + clientId + "'";
            Console.WriteLine(requestUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseContent);
            string subid = jo["value"][0]["id"].ToString();
            string subdisplayName = jo["value"][0]["displayName"].ToString();
            Console.WriteLine(subid);
            return subid;
        }

        public async Task<string> GetSubscriptions()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            string requestUrl = "https://graph.microsoft.com/v1.0/subscriptions";
            Console.WriteLine(requestUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            string subid = jo["value"][0]["id"].ToString();
            //string subdisplayName = jo["value"][0]["displayName"].ToString();
            Console.WriteLine(subid);
            return subid;
        }
    }
}
