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
            string authCode = data.authToken;
            string graphCode = data.GraphauthToken;
            string roleNameDelete = data.roleName;
            log.LogInformation(authCode);
            Console.WriteLine(authCode);


            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string redirecturi = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);


            //"baf1387d-a1ed-44d2-af1e-738a43985599", ")1$Z.D#/}((>&/Jt[*?{_)[L?}.]_^%&{)@;%", "https://azuredfwassacreation.z8.web.core.windows.net/login.html"

            // get Tenant ID
            OrganizationController getOrganization = new OrganizationController(graphCode);
            var tenantId = await getOrganization.GetTenantID();
            Console.WriteLine(tenantId);
            log.LogInformation("tenantId #################");
            log.LogInformation(tenantId);

            // get management api token
            ManagementLogin loginManager = new ManagementLogin(tenantId, "baf1387d-a1ed-44d2-af1e-738a43985599", ")1$Z.D#/}((>&/Jt[*?{_)[L?}.]_^%&{)@;%", "https://azuredfwassacreation.z8.web.core.windows.net/login.html");
            string accessTokenManager = await loginManager.ReturnManagementTokenAsync();
            Console.WriteLine(accessTokenManager);

            // Get Subscriptions
            SubscriptionsController subs = new SubscriptionsController(accessTokenManager);
            var tenantsubs = await subs.GetAllSubscriptionsAsync();
            int cspsubs = SubscriptionsController.filterResourceByName(tenantsubs);
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs[cspsubs]);

            // Switch Logic
            RbacControllers setRbac = new RbacControllers(accessTokenManager);

            // Delete Rbac ID -- https://docs.microsoft.com/en-us/azure/active-directory/roles/permissions-reference
            switch (roleNameDelete)
            {
                case "Owner":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "8e3af657-a8ff-443c-a75c-2fe8c4bcb635");
                    break;
                case "Contributor":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "b24988ac-6180-42a0-ab88-20f7382dd24c");
                    break;
                case "Global Reader":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "f2ef992c-3afb-46b9-b7cf-a126ee74c451");
                    break;
                case "User Administrator":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "fe930be7-5e62-47db-91af-98c3a49a38b1");
                    break;
                case "Billing Administrator":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "b0f54661-2d74-4c50-afa3-1ec803f12efe");
                    break;
                case "Application Administrator":
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3");
                    break;
                default:
                    setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], "8e3af657-a8ff-443c-a75c-2fe8c4bcb635");
                    break;
            }


            // Return Object
            var myObj = new { tenant =  tenantsubs[cspsubs], roleDelete = roleNameDelete };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);
        }
    }
}
