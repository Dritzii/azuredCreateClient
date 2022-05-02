using Newtonsoft.Json;
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
            JObject properties = (JObject)jo["properties"];
            var surveytrackingA = new JArray();
            JArray item = (JArray)properties["routes"];
            item.Remove("id");
            item.Remove("etag");
            foreach (var items in item)
            {
                if (items["properties"]["nextHopType"].ToString() == "Internet")
                {
                }
                else
                {
                    /*
                     * {
                        "name": "route1",
                        "properties": {
                          "addressPrefix": "101.0.3.0/24",
                          "nextHopType": "Internet"
                        }
                      }
                    var json = new JObject();
                    json.Add("id", "Luna");
                    json.Add("name", "Silver");
                    json.Add("age", 19);
                     */
                    var payload = new { name = items["name"].ToString(), properties = new { addressPrefix = items["properties"]["addressPrefix"].ToString(),
                        nextHopType = items["properties"]["nextHopType"].ToString() } };
                    surveytrackingA.Add(JsonConvert.SerializeObject(payload));
                }

            }
            return surveytrackingA.ToString();
        }
        public static int filterResourceByTag(List<string> listName)
        {
            int index = listName.FindIndex(a => a == "{\r\n  \"FWaaSAzured\": \"GatewaySubnetRoute\"\r\n}");
            return index - 1;

        }

        public static Boolean JarrayOver300(JArray array)
        {
            if (array.Count == 0)
            {
                if (array.Parent is JProperty && array.Parent.Parent != null)
                {
                    array.Parent.Remove();
                }
                else if (array.Parent is JArray)
                {
                    array.Remove();
                }
            }
            return false;
        }
    }
}
