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
    public static class CustomerLoginTrigger
    {
        [FunctionName("CustomerLoginTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string code = data.code;
            Console.WriteLine(code);
            log.LogInformation(code);

            // Trim Code String
            TrimStringFromUrl urlString = new TrimStringFromUrl(code);
            string responseMessage = urlString.ReturnCode();
            log.LogInformation(responseMessage);
            Console.WriteLine(responseMessage);

            // Get the Application details from the settings
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string redirecturi = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);
            Console.WriteLine(clientId);
            Console.WriteLine(clientSecret);
            log.LogInformation(clientId);
            log.LogInformation(redirecturi);
            log.LogInformation(clientSecret);
            // Get the access token from MS Identity
            ManagementLogin managementLogin = new ManagementLogin("common", clientId, clientSecret, redirecturi);
            var managementtoken = await managementLogin.CustomerReturnManagementTokenAsync(responseMessage);
            log.LogInformation(managementtoken.ToString());

            // test sub iteration

            //SubscriptionsController subscriptionsCon = new SubscriptionsController(accessToken);
            //var sublist = await subscriptionsCon.GetAllSubscriptionsAsync();

            var graphtoken = await managementLogin.RefreshReturnManagementTokenAsync(managementtoken[1]);
            log.LogInformation(graphtoken.ToString());

#pragma warning disable IDE0037 // Use inferred member name
            var myObj = new { accessToken = managementtoken[0], managementRefresh = managementtoken[1], graphToken = graphtoken[0] , graphRefresh = graphtoken[1] };
#pragma warning restore IDE0037 // Use inferred member name
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            return new JsonResult(jsonToReturn); // returning json
        }
    }
}
