using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            ManagementLogin loginManager = new ManagementLogin(tenantId, clientId, clientSecret, redirecturi);
            string accessTokenManager = await loginManager.ReturnManagementTokenAsync();
            Console.WriteLine(accessTokenManager);

            // Get Subscriptions
            SubscriptionsController subs = new SubscriptionsController(accessTokenManager);
            var tenantsubs = await subs.GetAllSubscriptionsAsync();
            int cspsubs = SubscriptionsController.filterResourceByName(tenantsubs);
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs[cspsubs]);
            
            // get Application Registration
            MicrosoftGraph msGraph = new MicrosoftGraph(graphCode);
            var objectId = await msGraph.GetObjectId(clientId);
            Console.WriteLine(objectId);
            log.LogInformation("objectId #################");
            log.LogInformation(objectId.ToString());

            // Switch Logic
            RbacControllers setRbac = new RbacControllers(accessTokenManager);
            var rbacScopedAssignments = await setRbac.GetSingleRoleAssignmentsForScope(tenantsubs[cspsubs], objectId);
            log.LogInformation(rbacScopedAssignments[0]);
            log.LogInformation(rbacScopedAssignments[1]);


            // Delete Rbac ID -- https://docs.microsoft.com/en-us/azure/active-directory/roles/permissions-reference
            setRbac.DeleteRbacSubscriptions(tenantsubs[cspsubs], rbacScopedAssignments[2]);
            log.LogInformation(JsonConvert.SerializeObject(new { tenant = tenantsubs[cspsubs]}));
            Console.WriteLine(JsonConvert.SerializeObject(new { tenant = tenantsubs[cspsubs]}));
            return new JsonResult(JsonConvert.SerializeObject(new { tenant = tenantsubs[cspsubs] }));

        }
    }
}
