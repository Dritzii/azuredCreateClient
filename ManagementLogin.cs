using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class ManagementLogin
    {
        readonly string baseUrl = "https://login.microsoftonline.com/";
        readonly string endPoint = "/oauth2/token";
        string clientId;
        string clientSecret;
        string tenantId;


        public ManagementLogin(string tenantId, string clientId, string clientSecret)
        {
            this.tenantId = tenantId;
            this.clientId = clientId;
            this.clientSecret = clientSecret;

        }

        public async Task<string> returnManagementTokenAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/x-www-form-urlencoded");
            string authenticationURL = "client_id=" + this.clientId + "&grant_type=client_credentials&resource=https://management.azure.com&client_secret=" + this.clientSecret;
            string tenantUrl = baseUrl + this.tenantId + endPoint;

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("resource", "https://management.azure.com"),
                new KeyValuePair<string, string>("client_secret", this.clientSecret)
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tenantUrl)
            {
                Content = new FormUrlEncodedContent(content),
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent); -- this returns a string cause deserialize lel
            //Console.WriteLine(responseObject);
            var jo = JObject.Parse(responseContent);
            var id = jo["access_token"].ToString();
            return id;
             
        }
    }
}
