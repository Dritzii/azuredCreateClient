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
    public static class addFirewall
    {
        [FunctionName("addFirewall")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.authToken;
            string graphCode = data.GraphauthToken;
            string firewall = data.firewall;
            log.LogInformation(authCode);
            log.LogInformation(graphCode);

            string connectionstring = Environment.GetEnvironmentVariable("connectionstring", EnvironmentVariableTarget.Process);
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);

            // get db connection
            DatabaseConnectioncs dbconn = new DatabaseConnectioncs(connectionstring);
            // "Server=arazured.database.windows.net,1433;Initial Catalog=fwaasapplication;User ID=aradmin;Password=Aqualite12@;"
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
            // filter rows from subs and get only CSP subs
            int cspsubs = SubscriptionsController.filterResourceByName(tenantsubs);
            int cspname = cspsubs - 1;
            // database logic
            var firewalldb = dbconn.firewallInDB(firewall);
            var subindb = dbconn.subInDb(tenantsubs[cspsubs]);
            log.LogInformation(subindb.ToString());
            log.LogInformation(tenantsubs[cspsubs].ToString());
            // true is null here
            if (firewalldb == true)
            {
                dbconn.InsertIntoFirewall(tenantsubs[cspsubs], firewall);
                log.LogInformation(tenantsubs[cspsubs].ToString());
                log.LogInformation(tenantsubs[cspname].ToString());
            }


            // Return Object
            var myObj = new { graphapiToken = graphCode, tenantid = tenantId, managementToken = authCode, subscriptionId = tenantsubs };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);
        }
    }
}
