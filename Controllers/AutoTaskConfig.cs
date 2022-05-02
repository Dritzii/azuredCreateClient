using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace azuredCreateClient.Controllers
{
    class AutoTaskConfig
    {
        private readonly string apicode;
        private readonly string username;
        private readonly string secret;
        private readonly string baseUrl = "https://webservices6.autotask.net/atservicesrest/v1.0/";

        public AutoTaskConfig(string apicode, string username, string secret)
        {
            this.apicode = apicode;
            this.username = username;
            this.secret = secret;
        }

        public async Task<int> GetCompanyId(string companyName)
        {
            string url = baseUrl + String.Format("/Companies/query?search={\"filter\":[{\"op\":\"like\",\"field\":\"CompanyName\",\"value\":\"{}\"}]}", companyName);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ApiIntegrationCode", this.apicode);
            client.DefaultRequestHeaders.Add("UserName", this.username);
            client.DefaultRequestHeaders.Add("Secret", this.secret);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            Console.WriteLine(url);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            int subid = (int)jo["items"][0]["id"];
            Console.WriteLine("Autotask Company ID " + subid);
            return subid;
        }

        public async void CreateTicket(int companyId = 409, string titleTicket = "TestCindy", int status = 5)
        {
            string url = baseUrl + "Tickets";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ApiIntegrationCode", this.apicode);
            client.DefaultRequestHeaders.Add("UserName", this.username);
            client.DefaultRequestHeaders.Add("Secret", this.secret);

            var payload = new { companyID = companyId, QueueID = 5, title = titleTicket, status = status , priority = 2};
            var jsonToReturn = JsonConvert.SerializeObject(payload);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonToReturn.ToString(), Encoding.UTF8, "application/json"),
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

        }
    }
}
