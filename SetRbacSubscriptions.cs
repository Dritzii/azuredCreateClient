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
        private static readonly string apiVersion = "?api-version=2015-07-01";
        public SetRbacSubscriptions(string Token)
        {
            this.Token = Token;
        }

        public async void PutRbacSubscriptions(string subscriptionId, string rbacName, string principalId)
        {

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            string joinedURL = hostUrl + subscriptionId + endpointUrl + rbacName + apiVersion;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            var bottomObject = new { roleDefinitionId = "/subscriptions/" + subscriptionId + "providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635", principalId = principalId };
            Console.WriteLine(bottomObject);
            var topObject = new { properties = bottomObject };
            Console.WriteLine(topObject);
            var jsonToReturn = JsonConvert.SerializeObject(topObject);
            Console.WriteLine(jsonToReturn);
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, joinedURL);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            Console.WriteLine(responseObject);
            //return responseObject;

        }
    }
}
