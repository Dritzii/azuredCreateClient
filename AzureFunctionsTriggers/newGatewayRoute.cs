using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using azuredCreateClient.Controllers;
using azuredCreateClient.Middleware;
using Newtonsoft.Json.Linq;

namespace azuredCreateClient.AzureFunctionsTriggers
{
    public static class newGatewayRoute
    {
        [FunctionName("newGatewayRoute")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the authentication code from the request payload
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string firewall = data.firewall;
            string ipaddress = data.ipaddress;

            // Get the Application details from the settings
            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string redirecturi = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);
            string connectionstring = Environment.GetEnvironmentVariable("connectionstring", EnvironmentVariableTarget.Process);
            string autotaskapicode = Environment.GetEnvironmentVariable("autotaskapicode", EnvironmentVariableTarget.Process);
            string autotaskuser = Environment.GetEnvironmentVariable("autotaskuser", EnvironmentVariableTarget.Process);
            string autotaskpass = Environment.GetEnvironmentVariable("autotaskpass", EnvironmentVariableTarget.Process);

            /*
            * "EG3FIQPCKDFLL5EY3TXYHMDMMCY", "apijp@AZURED.COM.AU", "q*1Z$2TxwC#7Q~6n@9gF8R*o@"
            * autotaskapicode, autotaskuser, autotaskpass
            */
            // Autotask Init
            AutoTaskConfig aconfig = new AutoTaskConfig(autotaskapicode, autotaskuser, autotaskpass);

            DatabaseConnectioncs dbconn = new DatabaseConnectioncs(connectionstring);
            // "Server=arazured.database.windows.net,1433;Initial Catalog=fwaasapplication;User ID=aradmin;Password=Aqualite12@;"
            var dbdata = dbconn.GetFirewallfromDB(firewall);
            Console.WriteLine("TENANT ID IS: " + dbdata[0].tenantId);

            // Get the access token from MS Identity
            /*
             * "baf1387d-a1ed-44d2-af1e-738a43985599", ")1$Z.D#/}((>&/Jt[*?{_)[L?}.]_^%&{)@;%", "https://azuredfwassacreation.z8.web.core.windows.net/login.html"
             *  clientId, clientSecret, redirecturi
             */
            ManagementLogin managementLogin = new ManagementLogin(dbdata[0].tenantId, clientId, clientSecret, redirecturi);
            var managementtoken = await managementLogin.ReturnManagementTokenAsync();
            log.LogInformation(managementtoken.ToString());

            AzureServicesController getresource = new AzureServicesController(managementtoken);
            var resourceData = await getresource.GetResourceByTag(dbdata[0].subscriptionId);
            //int indexList = JsonPlaying.filterResourceByTag(resourceData); -- removed as using get resource by tag has been updated to only match for fwaas tags - no need for this
            //getresource.NewGatewayRoute(resourceData[indexList], ipaddress); -- so the last of the 300 will get inserted at the finally block
            string routeData = await getresource.GetRouteTable(resourceData[0]);
            // Get All Routes from Table
            JArray allRoutesJarray = JsonPlaying.GetAllRoutesFromRouteTableToJarray(routeData);
            //If number of Routes exceed 300 then clear and add the non internet gateways in
            if (JsonPlaying.JarrayOverCount(allRoutesJarray) == true)
            {
                //  get previous routes and only get non internet ones
                JArray iterateRTJSON = JsonPlaying.GetListofRoutesFromTable(routeData);
                if (JsonPlaying.JarrayOverCount(iterateRTJSON, 0) == false)
                {
                    try
                    {
                        JObject payloadObject = JsonPlaying.NewGatewayRouteObject(ipaddress);
                        JArray finalMerged = JsonPlaying.AddToJArray(iterateRTJSON, payloadObject);
                        // update whole table with non internet ones
                        getresource.updateOrCreateRouteTableWithRoutes(resourceData[0], finalMerged);

                    }
                    catch (Exception)
                    {
                        // any kind of error, we create a ticket
                        int companyId = await aconfig.GetCompanyId(dbdata[0].uri);
                        aconfig.CreateTicket(companyId);
                        return new BadRequestObjectResult(String.Format("Ticket Created in Autotask for Company {0}", dbdata[0].uri)); // 400
                    }
                    //finally
                    //{
                    //System.Threading.Thread.Sleep(15000); //the above is more dynamic, no more waiting
                    //getresource.NewGatewayRoute(resourceData[indexList], ipaddress);
                    //}
                }
                else
                {
                    JObject payloadObject = JsonPlaying.NewGatewayRouteObject(ipaddress);
                    JArray finalMerged = JsonPlaying.JobjectIntoJarray(payloadObject);
                    // update whole table with non internet ones
                    getresource.updateOrCreateRouteTableWithRoutes(resourceData[0], finalMerged);
                }
            }
            else
            {
                // just add the one route if not over 300 array
                getresource.NewGatewayRoute(resourceData[0], ipaddress);
            }
            dbconn.InsertIntoHistory(dbdata[0].tenantId, "NMAgent-" + ipaddress, ipaddress, resourceData[0], dbdata[0].subscriptionId, dbdata[0].displayName, resourceData[0] + string.Format("/routes/{0}?api-version=2021-04-01", firewall), routeData);
            return new OkObjectResult(new { tenantId = dbdata[0].tenantId, ipaddressRTName = "NMAgent-" + ipaddress, ipaddress = ipaddress, resourcePath = resourceData[0], subscriptionId = dbdata[0].subscriptionId, subscriptionName = dbdata[0].displayName, fullResourcePath = resourceData[0] + string.Format("/routes/{0}?api-version=2021-04-01", firewall), payload = routeData }); // 200
        }
    }
}
