using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace azuredCreateClient
{
    class MicrosoftGraph
    {
        readonly RestClient _client;
        string Token;

        public MicrosoftGraph(string Token)
        {
            _client = new RestClient();
            this.Token = Token;
        }

        public async Task<string> GetMe()
        {
            var request = new RestRequest("https://graph.microsoft.com/v1.0/me");
            request.AddHeader("Authorization", "Bearer " + Token);
            var response = await _client.GetAsync(request);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Content);
            var DeserialresponseObject = JsonConvert.DeserializeObject(response.Content);
            return (string)DeserialresponseObject;
        }
    }
}
