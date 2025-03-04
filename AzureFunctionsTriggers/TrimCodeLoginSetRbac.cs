using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azuredCreateClient
{
    public static class TrimCodeLoginSetRbac
    {
        [FunctionName("TrimCodeLoginSetRbac")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.authToken;
            string graphCode = data.GraphauthToken;
            log.LogInformation(authCode);
            log.LogInformation(graphCode);
            //Console.WriteLine(authCode);

            // Trim Code String
            //TrimStringFromUrl urlString = new TrimStringFromUrl(authCode);
            //string responseMessage = urlString.ReturnCode();
            //log.LogInformation(responseMessage);
            //log.LogInformation("Auth Code String Cut #################");
            //Console.WriteLine(responseMessage);

            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            //string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            //ManagementLogin managementLogin = new ManagementLogin("common", "527d9552-ccf3-4a7e-a149-4a52363d3f55", "Wz57Q~fJ.uJ3pxx52e0BlmgLi9UZWjYs9DZP4");
            //var managementtoken = await managementLogin.CustomerReturnManagementTokenAsync(responseMessage);
            //log.LogInformation(managementtoken.ToString());

            // test sub iteration

            //SubscriptionsController subscriptionsCon = new SubscriptionsController(accessToken);
            //var sublist = await subscriptionsCon.GetAllSubscriptionsAsync();

            //var graphtoken = await managementLogin.RefreshReturnManagementTokenAsync(managementtoken[1]);
            //log.LogInformation(graphtoken.ToString());

#pragma warning disable IDE0037 // Use inferred member name
            //var myObj = new { accessToken = managementtoken[0], managementRefresh = managementtoken[1], graphToken = graphtoken[0], graphRefresh = graphtoken[1] };

            // Get the access token from MS Identity
            //MicrosoftIdentityClient idClient = new MicrosoftIdentityClient(clientId, clientSecret, "common");
            //string accessToken = await idClient.GetAccessTokenFromAuthorizationCode(responseMessage);
            //Console.WriteLine(accessToken);
            //log.LogInformation("Access TOken #################");
            //log.LogInformation(accessToken);

            // get Tenant ID
            OrganizationController getOrganization = new OrganizationController(graphCode);
            var tenantId  = await getOrganization.GetTenantID();
            Console.WriteLine(tenantId);
            log.LogInformation("tenantId #################");
            log.LogInformation(tenantId);

            // get management api token
            //ManagementLogin loginManager = new ManagementLogin(tenantId, clientId, clientSecret);
            //string accessTokenManager = await loginManager.ReturnManagementTokenAsync();
            //Console.WriteLine(accessTokenManager);
            //log.LogInformation("accessTokenManager #################");
            //log.LogInformation(accessTokenManager);

            

            // Get Subscriptions
            SubscriptionsController subs = new SubscriptionsController(authCode);
            var tenantsubs = await subs.GetAllSubscriptionsAsync();
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs);
            log.LogInformation("tenantsubs #################");
            log.LogInformation(tenantsubs.ToString());

            // Get Objectid of the Client application on Tenancy
            // https://graph.microsoft.com/v1.0/serviceprincipals?$filter=appId eq '{client id of your  application registration}'
            MicrosoftGraph msGraph = new MicrosoftGraph(graphCode);
            var objectId = await msGraph.GetObjectId(clientId);
            Console.WriteLine(objectId);
            log.LogInformation("objectId #################");
            log.LogInformation(objectId.ToString());


            // Add Rbac with new Guid
            RbacControllers setRbac = new RbacControllers(authCode);
            foreach (var tenantsub in tenantsubs)
            {
                // Create GUID
                GuidController newGuid = new GuidController();
                string newGuidReturned = newGuid.ReturnGuid().ToString();
                Console.WriteLine(newGuidReturned);
                log.LogInformation("newGuidReturned #################");
                log.LogInformation(newGuidReturned);
                setRbac.PutRbacSubscriptions(tenantsub, newGuidReturned, objectId);

            };
            //setRbac.PutRbacSubscriptions(tenantsubs, newGuidReturned, objectId); //"00e669b6-1cac-4ec1-b576-e59be8e23e2e"


            // Return Object
            var myObj = new { graphapiToken = graphCode, tenantid = tenantId, managementToken = authCode, subscriptionId = tenantsubs , serviceprincipalId = objectId};
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);

        }
    }
}
