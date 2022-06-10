using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using azuredCreateClient.Middleware;

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

            string connectionstring = Environment.GetEnvironmentVariable("connectionstring", EnvironmentVariableTarget.Process);
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);

            // get db connection
            DatabaseConnectioncs dbconn = new DatabaseConnectioncs(connectionstring);

            // get Tenant ID
            OrganizationController getOrganization = new OrganizationController(graphCode);
            var tenantId  = await getOrganization.GetTenantID();
            Console.WriteLine(tenantId);
            log.LogInformation("tenantId #################");
            log.LogInformation(tenantId);
            
            // Get Subscriptions
            SubscriptionsController subs = new SubscriptionsController(authCode);
            var tenantsubs = await subs.GetAllSubscriptionsAsync();
            Console.WriteLine("SUBSCRIPTION : " + tenantsubs);
            log.LogInformation("tenantsubs #################");
            log.LogInformation(tenantsubs.ToString());
            tenantsubs.ForEach(p => log.LogInformation(p));
            // filter rows from subs
            int cspsubs = SubscriptionsController.filterResourceByName(tenantsubs);
            int onlyidsub = cspsubs - 1;
            // database logic
            var tenantiddb = dbconn.tenantInDB(tenantId);
            log.LogInformation(tenantiddb.ToString());
            log.LogInformation(tenantId.ToString());
            var subindb = dbconn.subInDb(tenantsubs[cspsubs]);
            log.LogInformation(subindb.ToString());
            log.LogInformation(tenantsubs[cspsubs].ToString());
            Console.WriteLine("TenantSubs : " + tenantsubs[cspsubs].ToString());
            Console.WriteLine("ALl : " + tenantsubs[onlyidsub].ToString());
            // true is null here
            if (tenantiddb == true)
            {
                if(subindb == true)
                {
                    dbconn.InsertIntoTenantandSubscriptions(tenantsubs[cspsubs], tenantsubs[onlyidsub], tenantId);
                    log.LogInformation(tenantsubs[cspsubs].ToString());
                }
            }
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

            // Return Object
            var myObj = new { graphapiToken = graphCode, tenantid = tenantId, managementToken = authCode, subscriptionId = tenantsubs , serviceprincipalId = objectId};
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);

        }
    }
}
