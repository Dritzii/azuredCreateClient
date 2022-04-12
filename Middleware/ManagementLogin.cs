using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using azuredCreateClient.Middleware;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class ManagementLogin
    {
        readonly string baseUrl = "https://login.microsoftonline.com/";
        readonly string endPoint = "/oauth2/v2.0/token";
        readonly string redirecturi;
        readonly string clientId;
        readonly string clientSecret;
        readonly string tenantId;
        private FirewallClass firewallClass;

        public ManagementLogin(string tenantId, string clientId, string clientSecret, string redirecturi)
        {
            this.tenantId = tenantId;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.redirecturi = redirecturi;
        }

        public ManagementLogin(FirewallClass firewallClass, string clientId, string clientSecret, string redirecturi)
        {
            this.firewallClass = firewallClass;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.redirecturi = redirecturi;
        }

        public async Task<string> ReturnManagementTokenAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/x-www-form-urlencoded");
            //string authenticationURL = "client_id=" + this.clientId + "&grant_type=client_credentials&resource=https://management.azure.com&client_secret=" + this.clientSecret;
            string tenantUrl = baseUrl + this.tenantId + endPoint;

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("resource", "https://management.azure.com/.default"),
                new KeyValuePair<string, string>("client_secret", this.clientSecret)
            };
            Console.WriteLine(tenantUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tenantUrl)
            {
                Content = new FormUrlEncodedContent(content),
                
            };
            Console.WriteLine(content);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent); -- this returns a string cause deserialize lel
            //Console.WriteLine(responseObject);
            var jo = JObject.Parse(responseContent);
            var id = jo["access_token"].ToString();
            return id;
             
        }
        public async Task<List<string>> CustomerReturnManagementTokenAsync(string code)
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/x-www-form-urlencoded");
            //string authenticationURL = "client_id=" + this.clientId + "&grant_type=authorization_code&resource=https://management.azure.com&client_secret=" + this.clientSecret;
            string tenantUrl = baseUrl + this.tenantId + endPoint;

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_secret", this.clientSecret),
                new KeyValuePair<string, string>("redirect_uri", this.redirecturi),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("scope", "openid offline_access https://management.core.windows.net/.default")
            };
            Console.WriteLine(tenantUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tenantUrl)
            {
                Content = new FormUrlEncodedContent(content),

            };
            Console.WriteLine(content);
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
        public async Task<List<string>> RefreshReturnManagementTokenAsync(string code)
        {
            var retList = new List<string>();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/x-www-form-urlencoded");
            //string authenticationURL = "client_id=" + this.clientId + "&grant_type=authorization_code&resource=https://management.azure.com&client_secret=" + this.clientSecret;
            string tenantUrl = baseUrl + this.tenantId + endPoint;

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_secret", this.clientSecret),
                new KeyValuePair<string, string>("redirect_uri", this.redirecturi),
                new KeyValuePair<string, string>("refresh_token", code),
                new KeyValuePair<string, string>("scope", "openid offline_access https://graph.microsoft.com/.default")
            };
            Console.WriteLine(tenantUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tenantUrl)
            {
                Content = new FormUrlEncodedContent(content),

            };
            Console.WriteLine(content);
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
