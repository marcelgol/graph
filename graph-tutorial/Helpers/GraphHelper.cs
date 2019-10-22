// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using graph_tutorial.TokenStorage;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;
using graph_tutorial.Models;
using Newtonsoft.Json;

namespace graph_tutorial.Helpers
{
    public static class GraphHelper
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["ida:AppScopes"];

        public static string token = null;

        public static async Task<User> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            token = accessToken;

            return await graphClient.Me.Request().GetAsync();
        }

        public static async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var graphClient = GetAuthenticatedClient();

            var events = await graphClient.Me.Events.Request()
                .Select("subject,organizer,start,end")
                .OrderBy("createdDateTime DESC")
                .GetAsync();

            return events.CurrentPage;
        }

        public static async Task<IEnumerable<User>> GetUsersAsync()
        {
            var graphClient = GetAuthenticatedClient();

            var users = await graphClient.Users.Request().GetAsync();

           

            return users.CurrentPage;
        }

        public static async Task<ExcelChart> GetChart()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/me/drive/items/" + "01R5DLZY46KS3P4QDECNBJ4KLFMMXTOK5E" + "/workbook/worksheets('Sheet1')/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string chartId = null;

            HttpResponseMessage chartsResponse = await client.GetAsync("charts");

            var responseContent = await chartsResponse.Content.ReadAsStringAsync();
            var parsedResponse = JObject.Parse(responseContent);
            chartId = (string)parsedResponse["value"][0]["id"];

            HttpResponseMessage response = await client.GetAsync("charts('" + "Chart 2" + "')/Image(width=0,height=0,fittingMode='fit')");

          
            string resultString = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(resultString);
            
            var chart = new ExcelChart();
            chart.Image = result["value"].ToString();

            return chart;
        }



        private static GraphServiceClient GetAuthenticatedClient()
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();

                        var tokenStore = new SessionTokenStore(idClient.UserTokenCache, 
                            HttpContext.Current, ClaimsPrincipal.Current);

                        var accounts = await idClient.GetAccountsAsync();

                    // By calling this here, the token can be refreshed
                    // if it's expired right before the Graph call is made
                    var scopes = graphScopes.Split(' ');
                        var result = await idClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);


                        
                    }));
        }
    }
}