using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azuredCreateClient
{
    public class MicrosoftIdentityClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string hostUrl = "https://login.microsoftonline.com";

        private readonly string tenantId;
        private readonly string clientId;
        private readonly string clientSecret;

        public MicrosoftIdentityClient(string clientId, string clientSecret, string tenantId)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenantId = tenantId;
        }

        public async Task<string> GetAccessTokenFromAuthorizationCode(string authCode)
        {
            string redirectUrl = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);
            string scopes = Environment.GetEnvironmentVariable("scopes", EnvironmentVariableTarget.Process);

            Uri requestUri = new Uri($"{hostUrl}/{this.tenantId}/oauth2/v2.0/token");

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("scope", scopes),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl),
                new KeyValuePair<string, string>("client_secret", this.clientSecret)
                //new KeyValuePair<string, string>("resource", "https://management.azure.com")
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new FormUrlEncodedContent(content),
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

            if (response.IsSuccessStatusCode)
            {
                // dynamic values need to be assigned before passing back
                return (string)responseObject.access_token;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Something failed along the way, and there will be an error in there if the error code is 400
                // Handle it however you want.
                throw new Exception((string)responseObject.error_description);
            }
            else
            {
                // ¯\_(ツ)_/¯
                throw new Exception("Something bad happened");
            }
        }

        public async Task<string> GetAccessTokenFromAuthorizationCodeClientCredentials(string authCode, string tenantId)
        {
            string redirectUrl = Environment.GetEnvironmentVariable("redirecturi", EnvironmentVariableTarget.Process);
            string scopes = "openid offline_access https://graph.microsoft.com/.default";

            Uri requestUri = new Uri($"{hostUrl}/{tenantId}/oauth2/v2.0/token");

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", this.clientId),
                new KeyValuePair<string, string>("scope", scopes),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl),
                new KeyValuePair<string, string>("client_secret", this.clientSecret)
                //new KeyValuePair<string, string>("resource", "https://management.azure.com")
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new FormUrlEncodedContent(content),
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

            if (response.IsSuccessStatusCode)
            {
                // dynamic values need to be assigned before passing back
                return (string)responseObject.access_token;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Something failed along the way, and there will be an error in there if the error code is 400
                // Handle it however you want.
                throw new Exception((string)responseObject.error_description);
            }
            else
            {
                // ¯\_(ツ)_/¯
                throw new Exception("Something bad happened");
            }
        }
    }
}
