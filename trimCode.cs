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
    public static class TrimCode
    {
        [FunctionName("TrimCode")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.code;
            Console.WriteLine(authCode);
            TrimStringFromUrl urlString = new TrimStringFromUrl(authCode);
            string responseMessage = urlString.ReturnCode();
            return new OkObjectResult(responseMessage);
        }
    }
}
