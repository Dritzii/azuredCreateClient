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
            var responseMessage = await graphCall.GetMe();
            // Return Object
            var myObj = new { graphmedata = responseMessage };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);

        }
    }
}
