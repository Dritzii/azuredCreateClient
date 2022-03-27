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
    public static class ManagementLoginTrigger
    {
        [FunctionName("ManagementLoginTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string tenantId = data.tenantId;
            Console.WriteLine(tenantId);
            log.LogInformation(tenantId);

            // Get the Application details from the settings
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            // Get the access token from MS Identity
            ManagementLogin managementLogin = new ManagementLogin(tenantId, clientId, clientSecret);
            string accessToken = await managementLogin.returnManagementTokenAsync();
            log.LogInformation(accessToken);
            var myObj = new { accessToken = accessToken };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            return new JsonResult(jsonToReturn); // returning json
            
        }
    }
}
