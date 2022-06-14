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
            string maxRoutesCount = Environment.GetEnvironmentVariable("maxRoutesCount", EnvironmentVariableTarget.Process);

            /*
            * "EG3FIQPCKDFLL5EY3TXYHMDMMCY", "apijp@AZURED.COM.AU", "q*1Z$2TxwC#7Q~6n@9gF8R*o@"
            * autotaskapicode, autotaskuser, autotaskpass
            */
            // Autotask Init
            AutoTaskConfig aconfig = new AutoTaskConfig(autotaskapicode, autotaskuser, autotaskpass);

            DatabaseConnectioncs dbconn = new DatabaseConnectioncs(connectionstring);
            // "Server=arazured.database.windows.net,1433;Initial Catalog=fwaasapplication;User ID=aradmin;Password=Aqualite12@;"
            var dbdata = dbconn.GetFirewallfromDB(firewall);
            try
            {
                if (dbdata == null)
                {
                    var dbcompany = dbconn.GetAutotaskCompanyNameFromDB(firewall);
                    if (dbcompany == null)
                    {
                        return new BadRequestObjectResult(String.Format("No Company Name in database matches : ", firewall));
                    }
                    else
                    {
                        int companyId = await aconfig.GetCompanyId(dbcompany[0].C_LongName);
                        aconfig.CreateTicket(companyId, String.Format("Firewall not added for device because it is not on the database : ", firewall), 1, 2);
                        return new BadRequestObjectResult(String.Format("No firewall in database matches : ", firewall));
                    }
                }
                
            }
            catch (Exception E)
            {
                log.LogInformation(E.ToString());
                return new BadRequestObjectResult(String.Format("Database Error"));
            }
            log.LogInformation("firewall found on database - cont");
            log.LogInformation("TENANT ID IS: " + dbdata[0].tenantId);
            
            // Get the access token from MS Identity
            /*
             * "baf1387d-a1ed-44d2-af1e-738a43985599", ")1$Z.D#/}((>&/Jt[*?{_)[L?}.]_^%&{)@;%", "https://azuredfwassacreation.z8.web.core.windows.net/login.html"
             *  clientId, clientSecret, redirecturi
             */
            ManagementLogin managementLogin = new ManagementLogin(dbdata[0].tenantId, clientId, clientSecret, redirecturi);
            var managementtoken = await managementLogin.ReturnManagementTokenAsync();

            AzureServicesController getresource = new AzureServicesController(managementtoken);
            var resourceData = await getresource.GetResourceByTag(dbdata[0].subscriptionId);
            string routeData = await getresource.GetRouteTable(resourceData[0]);
            string routetableLocation = await getresource.GetRouteTableLocation(resourceData[0]);
            Console.WriteLine(routeData);

            // Get All Routes from Table

            JArray allRoutesJarray = JsonPlaying.GetAllRoutesFromRouteTableToJarray(routeData);
            int CountOfJarray = JsonPlaying.JarrayCount(allRoutesJarray);

            //If number of Routes exceed 300 then clear and add the non internet gateways in
            int maxRoutesInt = Int32.Parse(maxRoutesCount);
            log.LogInformation(String.Format("Filter table by : {0}", maxRoutesCount));
            if (JsonPlaying.JarrayOverCount(allRoutesJarray, maxRoutesInt) == true)
            {
                Console.WriteLine("Above 300");
                //  get previous routes and only get non internet ones
                JArray iterateRTJSON = JsonPlaying.GetListofRoutesFromTable(routeData);
                if (JsonPlaying.JarrayOverCount(iterateRTJSON, 0) == false)
                {
                    try
                    {
                        Console.WriteLine("Jarray Over 0 adding");
                        JObject payloadObject = JsonPlaying.NewGatewayRouteObject(ipaddress);
                        JArray finalMerged = JsonPlaying.AddToJArray(iterateRTJSON, payloadObject);
                        // update whole table with non internet ones
                        Console.WriteLine("Final Merged");
                        //Console.WriteLine(finalMerged);
                        getresource.updateOrCreateRouteTableWithRoutes(resourceData[0], finalMerged, routetableLocation);

                    }
                    catch (Exception e)
                    {
                        // any kind of error, we create a ticket
                        Console.WriteLine("Error");
                        Console.WriteLine(e);
                        var dbcompany = dbconn.GetAutotaskCompanyNameFromDB(firewall);
                        if (dbcompany == null)
                        {
                            Console.WriteLine("No Database with Company name");
                            return new BadRequestObjectResult(String.Format("Company not found in database in Autotask for Company")); // 400
                        }
                        else
                        {
                            int companyId = await aconfig.GetCompanyId(dbcompany[0].C_LongName);
                            aconfig.CreateTicket(companyId, String.Format("Firewall not added because of a unknown exception for device : ", firewall), 1, 2);
                            return new BadRequestObjectResult(String.Format("Ticket Created in Autotask for Company {0}", dbcompany[0].C_LongName)); // 400
                        }
                    }
                }
                else
                {
                    try
                    {
                        Console.WriteLine("Not Above 300");
                        JObject payloadObject = JsonPlaying.NewGatewayRouteObject(ipaddress);
                        JArray finalMerged = JsonPlaying.JobjectIntoJarray(payloadObject);
                        // update whole table with non internet ones
                        getresource.updateOrCreateRouteTableWithRoutes(resourceData[0], finalMerged, routetableLocation);
                    }
                    catch (Exception e)
                    {
                        // any kind of error, we create a ticket
                        Console.WriteLine("Error");
                        Console.WriteLine(e);
                        var dbcompany = dbconn.GetAutotaskCompanyNameFromDB(firewall);
                        if (dbcompany == null)
                        {
                            Console.WriteLine("No Database with Company name");
                            return new BadRequestObjectResult(String.Format("Company not found in database in Autotask for Company")); // 400
                        }
                        else
                        {
                            int companyId = await aconfig.GetCompanyId(dbcompany[0].C_LongName);
                            aconfig.CreateTicket(companyId, String.Format("Firewall not added because of a unknown exception for device : ", firewall), 1, 2);
                            return new BadRequestObjectResult(String.Format("Ticket Created in Autotask for Company {0}", dbcompany[0].C_LongName)); // 400
                        }
                    }
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("Adding 1 route only");
                    // just add the one route if not over 300 array
                    getresource.NewGatewayRoute(resourceData[0], ipaddress);
                }
                catch (Exception e)
                {
                    // any kind of error, we create a ticket
                    Console.WriteLine("Error");
                    Console.WriteLine(e);
                    var dbcompany = dbconn.GetAutotaskCompanyNameFromDB(firewall);
                    if (dbcompany == null)
                    {
                        Console.WriteLine("No Database with Company name");
                        return new BadRequestObjectResult(String.Format("Company not found in database in Autotask for Company")); // 400
                    }
                    else
                    {
                        int companyId = await aconfig.GetCompanyId(dbcompany[0].C_LongName);
                        aconfig.CreateTicket(companyId, String.Format("Firewall not added because of a unknown exception for device : ", firewall), 1, 2);
                        return new BadRequestObjectResult(String.Format("Ticket Created in Autotask for Company {0}", dbcompany[0].C_LongName)); // 400
                    }
                }
            }
            Console.WriteLine("Insert into History");
            dbconn.InsertIntoHistory(dbdata[0].tenantId, "NMAgent-" + ipaddress, ipaddress, resourceData[0], dbdata[0].subscriptionId, dbdata[0].displayName, resourceData[0] + string.Format("/routes/{0}?api-version=2021-04-01", firewall), CountOfJarray.ToString());
            return new OkObjectResult(new { tenantId = dbdata[0].tenantId, ipaddressRTName = "NMAgent-" + ipaddress, ipaddress = ipaddress, resourcePath = resourceData[0], subscriptionId = dbdata[0].subscriptionId, subscriptionName = dbdata[0].displayName, fullResourcePath = resourceData[0] + string.Format("/routes/{0}?api-version=2021-04-01", firewall), CountOfJarray = CountOfJarray }); // 200
        }
    }
}
