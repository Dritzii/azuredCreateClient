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
    public static class RemoveEnterpriseApplication
    {
        [FunctionName("RemoveEnterpriseApplication")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string graphCode = data.GraphauthToken;
            log.LogInformation(graphCode);

            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);

            // Get Objectid of the Client application on Tenancy
            // https://graph.microsoft.com/v1.0/serviceprincipals?$filter=appId eq '{client id of your  application registration}'
            MicrosoftGraph msGraph = new MicrosoftGraph(graphCode);
            var objectId = await msGraph.GetObjectId(clientId);
            msGraph.DeleteEnterpriseApplication(objectId);

            // Return Object
            var myObj = new { graphapiToken = graphCode,  serviceprincipalId = objectId };
            var jsonToReturn = JsonConvert.SerializeObject(myObj);
            log.LogInformation(jsonToReturn);
            Console.WriteLine(jsonToReturn);
            return new JsonResult(jsonToReturn);
        }
    }
}
