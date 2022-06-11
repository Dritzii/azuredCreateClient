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

namespace azuredCreateClient.AzureFunctionsTriggers
{
    public static class AddCompany
    {
        [FunctionName("AddCompany")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
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
            var tenantId = await getOrganization.GetTenantID();
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
                if (subindb == true)
                {
                    dbconn.InsertIntoTenantandSubscriptions(tenantsubs[cspsubs], tenantsubs[onlyidsub], tenantId);
                    log.LogInformation(tenantsubs[cspsubs].ToString());
                    var jsonReturnEntry = new { subscriptionid = tenantsubs[cspsubs], displayName = tenantsubs[onlyidsub], tenantId = tenantId };
                    var jsonwrapper = JsonConvert.SerializeObject(jsonReturnEntry);
                    return new JsonResult(jsonwrapper);
                }
            }
            else if (tenantiddb == false)
            {
                if (subindb == true)
                {
                    dbconn.InsertIntoTenantandSubscriptions(tenantsubs[cspsubs], tenantsubs[onlyidsub], tenantId);
                    log.LogInformation(tenantsubs[cspsubs].ToString());
                    var jsonReturnEntry = new { subscriptionid = tenantsubs[cspsubs], displayName = tenantsubs[onlyidsub], tenantId = tenantId };
                    var jsonwrapper = JsonConvert.SerializeObject(jsonReturnEntry);
                    return new JsonResult(jsonwrapper);
                }
            }
            else
            {

            }

            // Return Object
            return new OkObjectResult("no new Database entry");

        }
    }
}
