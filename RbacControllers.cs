using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient
{
    class RbacControllers
    {
        //private readonly HttpClient HttpClient;
        private static readonly string hostUrl = "https://management.azure.com/subscriptions/";
        private static readonly string endpointUrlRoleAssignments = "/providers/Microsoft.Authorization/roleAssignments/";
        private static readonly string endpointUrlRoleDefinitions = "/providers/Microsoft.Authorization/roleDefinitions/";
        private static readonly string getDefinitions = "/providers/Microsoft.Authorization/roleDefinitions";
        readonly string Token;
        string apiVersion = "?api-version=2015-07-01";
        readonly JsonSerializerSettings jss = new JsonSerializerSettings();

        public RbacControllers(string Token)
        {
            this.Token = Token;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public async void PutRbacSubscriptions(string subscriptionId, string rbacName, string principalId, string roleDefinitionId = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635")
        {

            using var client = new HttpClient();
            string joinedURL = hostUrl + subscriptionId + endpointUrlRoleAssignments + rbacName + apiVersion;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            Console.WriteLine(joinedURL);
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
        }

        public async void DeleteRbacSubscriptions(string subscriptionId, string roleDefinitionId = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635")
        {
            // DELETE https://management.azure.com/{scope}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentName}?api-version=2015-07-01
            using var client = new HttpClient();
            string joinedURL = hostUrl + subscriptionId + endpointUrlRoleAssignments + roleDefinitionId + apiVersion;
            Console.WriteLine(joinedURL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, joinedURL);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            //dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            Console.WriteLine(responseContent);
        }

        public async Task<string> GetRoleDefinitions(string subscriptionId, string roleName)
        {
            //GET https://management.azure.com/{scope}/providers/Microsoft.Authorization/roleDefinitions?$filter={$filter}&api-version=2015-07-01
            using var client = new HttpClient();
            string filterData = "$filter=roleName eq '" + roleName + "'";
            string joinedURL = hostUrl + subscriptionId + getDefinitions + "?&api-version=2015-07-01&" + filterData ;
            Console.WriteLine(joinedURL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, joinedURL);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var jo = JObject.Parse(responseContent);
            var id = jo["value"][0]["name"].ToString();
            return id;
        }
    }
}
