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
            string authCode = data.code;
            log.LogInformation(authCode);
            Console.WriteLine(authCode);

            TrimStringFromUrl urlString = new TrimStringFromUrl(authCode);
            string responseMessage = urlString.ReturnCode();
            log.LogInformation(responseMessage);
            Console.WriteLine(responseMessage);

            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            // Get the access token from MS Identity
            MicrosoftIdentityClient idClient = new MicrosoftIdentityClient(clientId, clientSecret, "common");
            string accessToken = await idClient.GetAccessTokenFromAuthorizationCode(responseMessage);
            Console.WriteLine(accessToken);
            log.LogInformation(accessToken);

            // get Tenant ID
            GetOrganization getOrganization = new GetOrganization(accessToken);
            var tenantId  = await getOrganization.GetTenantID();
            Console.WriteLine(tenantId);

            // get management api token
            ManagementLogin loginManager = new ManagementLogin(tenantId, clientId, clientSecret);
            string accessTokenManager = await loginManager.returnManagementTokenAsync();
            Console.WriteLine(accessTokenManager);

            // Create GUID
            CreateGuid newGuid = new CreateGuid();
            string newGuidReturned = newGuid.returnGuid().ToString();
            Console.WriteLine(newGuidReturned);

            // Get Subscriptions
            GetSubscriptions subs = new GetSubscriptions(accessTokenManager);
            string tenantsubs = subs.ToString();
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs);

            // Add Rbac with new Guid
            SetRbacSubscriptions setRbac = new SetRbacSubscriptions(accessTokenManager);
            setRbac.PutRbacSubscriptions(tenantsubs, newGuidReturned, "00e669b6-1cac-4ec1-b576-e59be8e23e2e");

            // Return Object
            var myObj = new { graphapiToken = accessToken, tenantid = tenantId, managementToken = accessTokenManager , guid = newGuidReturned , subscriptionId = tenantsubs };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);

        }
    }
}
