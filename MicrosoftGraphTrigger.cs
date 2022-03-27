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
    public static class MicrosoftGraphTrigger
    {
        [FunctionName("MicrosoftGraph")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.code;
            Console.WriteLine(authCode);
            MicrosoftGraph graphCall = new MicrosoftGraph(authCode);
            //var responseMessage = await graphCall.GetMe();
            var objectIdGrab = await graphCall.GetObjectId("80beb811-b1b9-46b9-8a1f-dd6be129f9fc", "00e669b6-1cac-4ec1-b576-e59be8e23e2e");
            Console.WriteLine(objectIdGrab);
            //GetSubscriptions subs = new GetSubscriptions(authCode);
            //var subscriptionslist = subs.GetAllSubscriptionsAsync();
            //Console.WriteLine(subscriptionslist);
            // above uses management token not the graph token which is why the above won't work
            return new OkObjectResult(objectIdGrab);

        }
    }
}
