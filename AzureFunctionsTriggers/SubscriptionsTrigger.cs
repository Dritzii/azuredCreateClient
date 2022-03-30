using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azuredCreateClient.AzureFunctionsTriggers
{
    public static class SubscriptionsTrigger
    {
        [FunctionName("SubscriptionsTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string accessToken = data.accessToken;
            Console.WriteLine(accessToken);
            log.LogInformation(accessToken);

            SubscriptionsController subscriptionsCon = new SubscriptionsController(accessToken);
            var sublist = await subscriptionsCon.GetAllSubscriptionsAsync();


#pragma warning disable IDE0037 // Use inferred member name
            var myObj = new { subsdata = sublist };
#pragma warning restore IDE0037 // Use inferred member name
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            return new JsonResult(jsonToReturn); // returning json
        }
    }
}
