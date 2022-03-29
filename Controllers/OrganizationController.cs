using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class OrganizationController
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://graph.microsoft.com/v1.0/organization";
        readonly string Token;
        readonly JsonSerializerSettings jss = new JsonSerializerSettings();

        public OrganizationController(string Token)
        {
            this.Token = Token;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public async Task<string> GetTenantID()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            //Console.WriteLine(this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, hostUrl);
            Console.WriteLine(hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            //Console.WriteLine(responseObject);
            var jo = JObject.Parse(responseContent);
            var id = jo["value"][0]["id"].ToString();
            Console.WriteLine(id);
            return id;

        }

    }
}
