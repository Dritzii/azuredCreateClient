using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using azuredCreateClient.Controllers;
using azuredCreateClient.Middleware;

namespace azuredCreateClient.AzureFunctionsTriggers
{
    public static class newGatewayRoute
    {
        [FunctionName("newGatewayRoute")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string tenantId = data.tenantId;
            string firewall = data.firewall;
            Console.WriteLine(tenantId);
            log.LogInformation(tenantId);

            // Get the Application details from the settings
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string redirecturi = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);
            string dbuser = Environment.GetEnvironmentVariable("dbuser", EnvironmentVariableTarget.Process);
            string dbhost = Environment.GetEnvironmentVariable("dbhost", EnvironmentVariableTarget.Process);
            string dbpass = Environment.GetEnvironmentVariable("dbpass", EnvironmentVariableTarget.Process);
            string dbase = Environment.GetEnvironmentVariable("dbase", EnvironmentVariableTarget.Process);
            log.LogInformation(clientId);
            log.LogInformation(clientSecret);
            log.LogInformation(redirecturi);
            Console.WriteLine(clientId);
            Console.WriteLine(clientSecret);
            Console.WriteLine(redirecturi);
            // Get the access token from MS Identity
            ManagementLogin managementLogin = new ManagementLogin(tenantId, clientId, clientSecret, redirecturi);
            var managementtoken = await managementLogin.ReturnManagementTokenAsync();
            log.LogInformation(managementtoken.ToString());

            DatabaseConnectioncs firewallData = new DatabaseConnectioncs(dbhost, dbuser, dbpass, dbase);
            var dbdata = firewallData.GetFirewallfromDB(firewall);

            AzureServicesController getresource = new AzureServicesController(managementtoken);
            var resourceData = getresource.GetResourceByTag((string)dbdata[0]);
#pragma warning disable IDE0037 // Use inferred member name
            var myObj = new { accessToken = managementtoken[0]};
#pragma warning restore IDE0037 // Use inferred member name
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            return new JsonResult(jsonToReturn); // returning json
        }
    }
}
