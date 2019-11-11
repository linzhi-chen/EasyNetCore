using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Common.Configuration
{
    public class OAuthConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        /// <summary>
        /// 认证服务器地址
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// 标识名称
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        public bool HttpsRequired { get; set; }

        /// <summary>
        /// 允许访问的资源
        /// </summary>
        public string AllowedScopes { get; set; }

        public string RedirectUris { get; set; }

        public string IdentityServerUri { get; set; }

        /// <summary>
        /// 是否开启密码授权模式
        /// </summary>
        public bool PasswordAuthorizationEnabled { get; set; }

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }

        public IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>() {
                new ApiResource(Audience,DisplayName)
            };
        }

        public IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                //客户端模式
                new Client{
                    ClientId=$"{nameof(GrantTypes.ClientCredentials)}_{ClientId}",
                    ClientSecrets={ new Secret(ClientSecret.Sha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={  AllowedScopes }
                },
                //密码模式
                 new Client{
                    ClientId=$"{nameof(GrantTypes.ResourceOwnerPassword)}_{ClientId}",
                    ClientSecrets={ new Secret(ClientSecret.Sha256()) },
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={  AllowedScopes }
                },
                 //简单模式
                new Client
                {
                    ClientId=$"{nameof(GrantTypes.Implicit)}_{ClientId}",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    AllowedScopes={ AllowedScopes },
                    RedirectUris={RedirectUris},
                    AllowAccessTokensViaBrowser=true
                },
                //授权码模式
                 new Client
                {
                    ClientId=$"{nameof(GrantTypes.Code)}_{ClientId}",
                    ClientSecrets={ new Secret(ClientSecret.Sha256()) },
                    AllowedGrantTypes=GrantTypes.Code,
                    AllowedScopes={ AllowedScopes },
                    RedirectUris={RedirectUris}
                }
            };
        }
    }
}
