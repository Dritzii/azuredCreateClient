using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace azuredCreateClient
{
    class SetRbacSubscriptions
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://management.azure.com/subscriptions/";
        private static readonly string endpointUrlRoleAssignments = "/providers/Microsoft.Authorization/roleAssignments/";
        private static readonly string endpointUrlRoleDefinitions = "/providers/Microsoft.Authorization/roleDefinitions/";
        private static readonly string getDefinitions = "/providers/Microsoft.Authorization/roleDefinitions?";
        string Token;
        private static readonly string apiVersion = "?api-version=2015-07-01";
        public SetRbacSubscriptions(string Token)
        {
            this.Token = Token;
        }

        public async void PutRbacSubscriptions(string subscriptionId, string rbacName, string principalId, string roleDefinitionId = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635")
        {

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            string joinedURL = hostUrl + subscriptionId + endpointUrlRoleAssignments + rbacName + apiVersion;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            var bottomObject = new { roleDefinitionId = "/subscriptions/" + subscriptionId + endpointUrlRoleDefinitions + roleDefinitionId, principalId = principalId }; // change role assignment for different role
            Console.WriteLine(bottomObject);
            var topObject = new { properties = bottomObject };
            Console.WriteLine(topObject);
            var jsonToReturn = JsonConvert.SerializeObject(topObject);
            Console.WriteLine(jsonToReturn);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, joinedURL) {
                Content = new StringContent(jsonToReturn.ToString(), Encoding.UTF8, "application/json"),
            };
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            Console.WriteLine(responseObject);
        }

        public async Task<string> GetRoleDefinitions(string subscriptionId, string roleName)
        {
            //https://management.azure.com/subscriptions/f11289f1-9fab-48b7-89b7-ca236d9dd931/providers/Microsoft.Authorization/roleDefinitions?api-version=2015-07-01
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using var client = new HttpClient();
            string filterData = "$filter=roleName eq '" + roleName + "'";
            string joinedURL = hostUrl + subscriptionId + getDefinitions + filterData + apiVersion;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, joinedURL);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
            Console.WriteLine(responseObject);
            return responseObject;

        }
    }
}
