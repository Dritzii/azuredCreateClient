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
    public static class DeleteRbacFromSubscriptioncs
    {
        [FunctionName("DeleteRbacFromSubscriptioncs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.code;
            string roleNameDelete = data.roleName;
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
            OrganizationController getOrganization = new OrganizationController(accessToken);
            var tenantId = await getOrganization.GetTenantID();
            Console.WriteLine(tenantId);

            // get management api token
            ManagementLogin loginManager = new ManagementLogin(tenantId, clientId, clientSecret);
            string accessTokenManager = await loginManager.ReturnManagementTokenAsync();
            Console.WriteLine(accessTokenManager);

            // Get Subscriptions
            SubscriptionsController subs = new SubscriptionsController(accessTokenManager);
            var tenantsubs = await subs.GetAllSubscriptionsAsync();
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs);

            // Delete Rbac ID
            RbacControllers setRbac = new RbacControllers(accessTokenManager);
            //string rbacname = await setRbac.DeleteRbacSubscriptions(tenantsubs, roleNameDelete);

            // Delete Rbac from Subscription
            //setRbac.DeleteRbacSubscriptions(tenantsubs, rbacname);

            // Return Object
            var myObj = new { graphapiToken = accessToken, tenantid = tenantId, managementToken = accessTokenManager, subscriptionId = tenantsubs};
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);
        }
    }
}
