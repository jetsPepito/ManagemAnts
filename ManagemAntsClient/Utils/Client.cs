using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ManagemAntsClient.Utils
{
    public class Client
    {
        private static string Url = "https://localhost:44352/api/";


        public static HttpClient SetUpClient(string endpoint)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(Url + endpoint);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            }
            catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("SetUpClient");
                errorWriter.WriteLine(e.Message);
                return null;
            }
        }

        public static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage postRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await client.SendAsync(postRequest);
            }
            catch (Exception e)
            {
                response = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadGateway, Content = JsonContent.Create(e) };
            }

            return response;
        }


        public static async Task<HttpResponseMessage> GetAsync(HttpClient client, string sendRequest)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await client.GetAsync(sendRequest);
            }
            catch (Exception e)
            {
                response = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.BadRequest, Content = JsonContent.Create(e) };
            }

            return response;
        }
    }
}
