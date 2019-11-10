using CLF.Service.DTO.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Xunit;
using Newtonsoft.Json;
using System;
using CLF.WebApi;
using System.Net.Http.Headers;

namespace CLF.Test
{
   public  class AccountApiControllerTest
    {
        private  static   HttpClient _client;
        public AccountApiControllerTest()
        {
            var builder = new WebHostBuilder()
               .ConfigureAppConfiguration(config =>
               {
                   config.SetBasePath(Directory.GetCurrentDirectory());
                   config.AddJsonFile("appsettings.json");
               })
               .UseStartup<Startup>();

            var testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }

        [Fact]
        public  async void Test_GenerateJwtAccessToken()
        {
            try {
                string userName = "chenlinfei929@126.com";
                string password = "qwert123";
                string action = "api/account/login?userName=" + userName + "&password=" + password;
                var result =  _client.PostAsJsonAsync(action, "").Result;
                var data = await result.Content.ReadAsStringAsync();
                result.EnsureSuccessStatusCode();
            }
            catch( Exception ex)
            {
                throw ex;
            }
        }

        [Fact]
        public  async void Test_PermissionApi()
        {
            string userName = "chenlinfei929@126.com";
            string password = "qwert123";
            string getTokenUrl = "api/account/login?userName=" + userName + "&password=" + password;
            var result = _client.PostAsJsonAsync(getTokenUrl, "").Result;
            var data = await result.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();

            var jsonData = JsonConvert.DeserializeObject<jwtResult>(data);

            string testAPi = "api/permission/GetPermission";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jsonData.token);
            var result1 = _client.GetAsync(testAPi).Result;
            var data1 = await result1.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();
        }
        

        public class jwtResult
        {
            public bool  success { get; set; }
            public string token { get; set; }
            public string refreshToken { get; set; }
        }
    }
}
