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
            
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);

            // Get the access token from MS Identity
            MicrosoftIdentityClient idClient = new MicrosoftIdentityClient(clientId, clientSecret, "common");
            string accessToken = await idClient.GetAccessTokenFromAuthorizationCode(responseMessage);
            Console.WriteLine(accessToken);

            // get Tenant ID
            GetOrganization getOrganization = new GetOrganization(accessToken);
            var tenantId  = await getOrganization.GetTenantID();
            var myObj = new { code = accessToken , tenantid = tenantId};
            var jsonToReturn = JsonConvert.SerializeObject(myObj);

            return new JsonResult(jsonToReturn);
            //return new JsonResult(accessToken);


        }
    }
}
