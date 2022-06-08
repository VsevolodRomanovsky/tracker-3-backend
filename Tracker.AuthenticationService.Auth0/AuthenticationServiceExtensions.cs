using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.ManagementApi;
using Tracker.AuthenticationService.Auth0.Abstractions;
using Tracker.AuthenticationService.Auth0.ConfigObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tracker.AuthenticationService.Auth0
{
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// This method will configure the services collection with the necessary setup details for Auth0 authentication
        /// using the Auth0 Config object.
        /// </summary>
        /// <param name="services">Services Collection</param>
        /// <param name="config"><see cref="Auth0Config"/></param>
        public static void AddAuthenticationWithAuth0(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Auth0Config>(configuration.GetSection(Auth0Config.Auth0));
            services.AddSingleton<IAuthenticationConnection, HttpClientAuthenticationConnection>();
            services.AddSingleton<IManagementConnection, HttpClientManagementConnection>();
            services.AddScoped<IAuth0ClientFactory, Auth0ClientFactory>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var config = configuration.GetSection(Auth0Config.Auth0).Get<Auth0Config>();
                    options.Authority = config.Authority;
                    options.Audience = config.Audience;
                    options.RequireHttpsMetadata = false;

                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            if (!(context.SecurityToken is JwtSecurityToken token)) return Task.CompletedTask;
                            if (context.Principal?.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", token.RawData));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }


        /// <summary>
        /// This will instruct your application to add permission claims to the default identity for each request.  This
        /// is required to allow for Permission Based Authorization.  This call must be made before calling UseAuthorization
        /// in the ASP.Net pipeline.  It also requires that the Services Collection contain a valid <see cref="IRoleValidator"/>
        /// instance.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>
        /// <param name="permissions">List of Application Permissions</param>
        public static void UsePermissions(this IApplicationBuilder app, List<string> permissions)
        {
            app.Use(async (context, next) =>
            {
                var userIdentity = context.User.Identities.FirstOrDefault();

                if (userIdentity != null)
                {
                    var permissionValidator =
                        context.RequestServices.GetService(typeof(IPermissionValidator)) as IPermissionValidator;

                    foreach (var permission in permissions.Where(permission =>
                        permissionValidator?.CurrentUserHasPermission(userIdentity, permission) == true))
                    {
                        userIdentity.AddClaim(new Claim("permission", permission));
                    }
                }

                await next.Invoke();
            });
        }

    }
}