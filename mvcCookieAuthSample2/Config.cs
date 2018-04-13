using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace mvcCookieAuthSample
{
    public class Config
    {

        public static IEnumerable<Client> GetClients()
        {

            return new List<Client> {
                // new Client()
                // {
                //     ClientId="client",
                //     AllowedGrantTypes=GrantTypes.ClientCredentials, //Client
                //     ClientSecrets = {
                //         new Secret("secret".Sha256())
                //     },
                //     AllowedScopes ={"api"}

                // },
                //  new Client()
                // {
                //     ClientId="pwd_client",
                //     AllowedGrantTypes=GrantTypes.ResourceOwnerPassword, //Password
                //     ClientSecrets = {
                //         new Secret("secret".Sha256())
                //     },
                //     RequireClientSecret =false, // 不需要提供ClientSecret 
                //     AllowedScopes ={"api"}

                // }
                 new Client()
                {
                    ClientId="mvc_client",
                    AllowedGrantTypes= GrantTypes.HybridAndClientCredentials,//GrantTypes.Implicit, // Implicit 
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                     //RequireClientSecret =false, // 不需要提供ClientSecret 

                    RedirectUris ={ "http://localhost:5003/signin-oidc"}, //client跳转url 生产环境是在数据库中，这里使用硬编码
                    PostLogoutRedirectUris ={ "http://localhost:5003/signout-callback-oidc"}, //退出时，返回到这个url
                    // RequireConsent = false,
                    RequireConsent = true, // 用户点击“同意”按钮的一个过程
                    #region for Consent screen
                    ClientName = "Mvc client Name",
                    ClientUri ="http://localhost:5003",
                    LogoUri ="https://stackify.com/wp-content/uploads/2017/10/NET-core-2.1-1-793x397.png",
                    AllowRememberConsent =true,
	                #endregion
                   
                    AlwaysIncludeUserClaimsInIdToken=true,

                    // scopes that client has access to
                     AllowedScopes ={
                         IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                         IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServer4.IdentityServerConstants.StandardScopes.Email
                     }

                }


             };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {

            return new List<ApiResource>
            {
                new ApiResource("mvcapi","My mvc api application") // 每一条ApiResource 有多个scope
            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {

            return new List<IdentityResource>{
                 new IdentityResources.OpenId(), //每一条IdentityResource对应一个scope
                 new IdentityResources.Email(),
                 new IdentityResources.Profile()

            };
        }

        public static List<TestUser> GetTestUsers()
        {

            return new List<TestUser>{
                new TestUser{

                    SubjectId="SubjectId002",
                    Username ="level2",
                    Password ="123456",
                    Claims = new List<Claim>{
                        new Claim("name","Level_Claim"),
                        new Claim("website","github.wxlevel.com")
                    }
                }
            };
        }


    }


}