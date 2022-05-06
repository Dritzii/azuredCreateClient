using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    var payload = new
                    {
                        name = items["name"].ToString(),
                        properties = new
                        {
                            addressPrefix = items["properties"]["addressPrefix"].ToString(),
                            nextHopType = items["properties"]["nextHopType"].ToString()
                        }
                    };
                    var jsonToReturn = JsonConvert.SerializeObject(payload);
                    string jsonPayload = jsonToReturn.ToString();
                    JObject JOpayload = JObject.Parse(jsonPayload);
                    //surveytrackingA.Add(JObject.FromObject(payload));
                    surveytrackingA.Add(JOpayload);
                }

            }
            return surveytrackingA;
        }
        public static JArray GetAllRoutesFromRouteTableToJarray(string jsonRT)
        {
            JObject jo = JObject.Parse(jsonRT);
            JObject properties = (JObject)jo["properties"];
            var surveytrackingA = new JArray();
            JArray item = (JArray)properties["routes"];
            item.Remove("id");
            item.Remove("etag");
            foreach (var items in item)
            {
                var payload = new
                {
                    name = items["name"].ToString(),
                    properties = new
                    {
                        addressPrefix = items["properties"]["addressPrefix"].ToString(),
                        nextHopType = items["properties"]["nextHopType"].ToString()
                    }
                };
                surveytrackingA.Add(JObject.FromObject(payload));
            }
            return surveytrackingA;
        }
        public static JObject NewGatewayRouteObject(string ipaddress, string profixName = "NMAgent-", string prefix = "/32", string nexthoptype = "internet")
        {
            var payload = new { name = profixName + ipaddress, properties = new { addressPrefix = ipaddress + prefix, nextHopType = nexthoptype } };
            return JObject.FromObject(payload);
        }

        public static JArray MergeJArray(JArray jarray1, JArray jarray2)
        {
            JArray jarrayInitial = jarray1;
            JArray jarrayIntoInitial = jarray2;
            JArray mergedJarray = new JArray(jarrayInitial.Union(jarrayIntoInitial));
            return mergedJarray;
        }
        public static JArray JobjectIntoJarray(JObject JObject)
        {
            JArray jarrayInitial = new JArray();
            jarrayInitial.Add(JObject);
            return jarrayInitial;
        }

        public static JArray AddToJArray(JArray jarray1, JObject JObject)
        {
            JArray jarrayInitial = jarray1;
            jarrayInitial.Add(JObject);
            return jarrayInitial;
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

        public static Boolean JarrayEmpty(JArray array)
        {
            if (array.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
