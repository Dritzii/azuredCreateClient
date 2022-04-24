using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace azuredCreateClient.Middleware
{
    class JsonPlaying
    {
        public static string GetListofRoutesFromTable(string jsonRT)
        {
            JObject jo = JObject.Parse(jsonRT);
            JObject channel = (JObject)jo["properties"];
            JArray surveytrackingA = new JArray();
            JArray item = (JArray)channel["routes"];
            //item.RemoveAll();
            foreach (var items in item)
            {
                if (items["properties"]["nextHopType"].ToString() == "Internet")
                {
                }
                else
                {
                    surveytrackingA.Add(items);
           
                }

            }
            //Console.Write(surveytrackingA.ToString());
            return surveytrackingA.ToString();
        }
        public static int filterResourceByTag(List<string> listName)
        {
            int index = listName.FindIndex(a => a == "{\r\n  \"FWaaSAzured\": \"GatewaySubnetRoute\"\r\n}");
            return index - 1;

        }
    }
}
