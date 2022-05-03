using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace azuredCreateClient.Middleware
{
    class JsonPlaying
    {
        public static JArray GetListofRoutesFromTable(string jsonRT)
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
                    answered my own question:
                    https://stackoverflow.com/questions/72082275/c-sharp-jarray-strings-to-jarray-of-objects/72082507?noredirect=1#comment127364153_72082507
                     */
                    var payload = new { name = items["name"].ToString(), properties = new { addressPrefix = items["properties"]["addressPrefix"].ToString(),
                        nextHopType = items["properties"]["nextHopType"].ToString() } };
                    var jsonToReturn = JsonConvert.SerializeObject(payload);
                    string jsonPayload = jsonToReturn.ToString();
                    JObject JOpayload = JObject.Parse(jsonPayload);
                    surveytrackingA.Add(JOpayload);
                }

            }
            return surveytrackingA;
        }
        public static int filterResourceByTag(List<string> listName)
        {
            int index = listName.FindIndex(a => a == "{\r\n  \"FWaaSAzured\": \"GatewaySubnetRoute\"\r\n}");
            return index - 1;
        }

        public static Boolean JarrayOver300(JArray array)
        {
            if (array.Count >= 300)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
