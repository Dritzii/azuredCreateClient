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
    public static class TrimCodeLogin
    {
        [FunctionName("TrimCodeLogin")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string authCode = data.code;

            TrimStringFromUrl urlString = new TrimStringFromUrl(authCode);
            string responseMessage = urlString.ReturnCode();
            string tenantId = "common";
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            // Get the access token from MS Identity
            MicrosoftIdentityClient idClient = new MicrosoftIdentityClient(clientId, clientSecret, tenantId);
            string accessToken = await idClient.GetAccessTokenFromAuthorizationCode(responseMessage);
            Console.WriteLine(accessToken);
            var myObj = new { code = accessToken };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            return new JsonResult(jsonToReturn);

            
        }
    }
}
