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
using Microsoft.AspNetCore.Identity;
using CLF.Model.Account;
using CLF.Common.Infrastructure;
using System.Threading.Tasks;
using System.Web;

namespace CLF.Test
{
    public   class AccountControllerTest
    {
        private readonly HttpClient _client;
        public AccountControllerTest()
        {
            var builder = new WebHostBuilder()
               .ConfigureAppConfiguration(config =>
               {
                   config.SetBasePath(Directory.GetCurrentDirectory());
                   config.AddJsonFile("appsettings.json");
               })
               .UseStartup<CLF.Web.Mvc.Startup>();

            var testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }

        /// <summary>
        /// 测试controller记得RegisterDTO参数前加[FromBody]，否则无法自动赋值
        /// 发送注册邮件需要更改appsetting邮件节点配置（username和password）
        /// </summary>
        [Fact]
        public  async void Test_Register()
        {
            try
            {
                string action = "/account/register";
                RegisterDTO model = new RegisterDTO
                {
                    Email = "chenlinfei929@126.com",
                    Password = "qwert123",
                    ConfirmPassword = "qwert123"
                };
                var result = await _client.PostAsJsonAsync(action, model);
                result.EnsureSuccessStatusCode();

                var data = await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 测试controller记得SignInDTO参数前加[FromBody]，否则无法自动赋值
        /// </summary>
        [Fact]
        public async void Test_Login()
        {
            string action = "/account/login?returnUrl=http://localhost:50995/";
            SignInDTO model = new SignInDTO
            {
                UserName = "chenlinfei929@126.com",
                Password = "qwert123",
            };
            var result = await _client.PostAsJsonAsync(action, model);
            result.EnsureSuccessStatusCode();

            var data = await result.Content.ReadAsStringAsync();
        }

        [Fact]
        public async void Test_ConfirmEmail()
        {
            string email = "chenlinfei929@126.com";
            string code = "CfDJ8EKIY007RwJBqalOXafqxSLF8mX%252bPXKWTsEJomeeVkGha3BXHH9bS0Ki3YflGKfeg8vtj64P2dOOtiigAKqZzs%252f9xbaTwDctB5okDC%252bCm1%252bSX0NIHuHtpPVWaYmnOcnk05vmht9l8Hz5jXRhdXeY%252fVZhKRZ4gtVtOdfzyhAhOHmO56vXthaBssQ7NNgUEBOZt1O1IuQLYrNTZowsKaogu%252fu3k6HEYQbY9PSs63w2J6AM%252bDLHUrv9lxtmjjWAXnqhmg%253d%253d";
            string action = "/account/confirmEmail?email=" + email + "&code=" + code;
            var result = await _client.GetAsync(action);
            result.EnsureSuccessStatusCode();

            var data = await result.Content.ReadAsStringAsync();
        }
        [Fact]
        public async void Test_GenerateUserTokenAsync()
        {
            string email = "chenlinfei929@126.com";
            string action = "/account/generateUserTokenAsync?email="+email;
            var result = await _client.GetAsync(action);
            result.EnsureSuccessStatusCode();

            var data = await result.Content.ReadAsStringAsync();
          }
    }
}
