using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace azuredCreateClient
{
    class SetRbacSubscriptions
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://management.azure.com/subscriptions/";
        private static readonly string endpointUrl = "/providers/Microsoft.Authorization/roleAssignments/";
        string Token;
        string subscriptionId;
        string rbacName;
        string apiVersion = "?api-version=2015-07-01";
        public SetRbacSubscriptions(string Token)
        {
            this.Token = Token;
        }

        public async Task<string> GetAllSubscriptionsAsync(string subscriptionId, string rbacName)
        {

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            
            Console.WriteLine(this.Token);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, hostUrl);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;

        }
    }
}
