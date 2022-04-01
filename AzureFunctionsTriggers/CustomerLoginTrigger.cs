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
            Console.WriteLine(clientId);
            Console.WriteLine(clientSecret);
            // Get the access token from MS Identity
            ManagementLogin managementLogin = new ManagementLogin("common", clientId, clientSecret);
            var token = await managementLogin.CustomerReturnManagementTokenAsync(responseMessage);
            log.LogInformation(token.ToString());

            // test sub iteration

            //SubscriptionsController subscriptionsCon = new SubscriptionsController(accessToken);
            //var sublist = await subscriptionsCon.GetAllSubscriptionsAsync();


#pragma warning disable IDE0037 // Use inferred member name
            var myObj = new { accessToken = token };
#pragma warning restore IDE0037 // Use inferred member name
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            return new JsonResult(jsonToReturn); // returning json
        }
    }
}
