using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;
using Tracker.AuthenticationService.Auth0;
using Tracker.AuthenticationService.Auth0.ConfigObjects;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Auth0Config _config;
        private readonly IAuth0ClientFactory _auth0ClientFactory;

        public AuthenticationService(IOptions<Auth0Config> config, IAuth0ClientFactory auth0ClientFactory)
        {
            _config = config.Value;
            _auth0ClientFactory = auth0ClientFactory;
        }

        public async Task<User> CreateUser(LoginDto loginDto)
        {
            using ManagementApiClient managementApiClient = await _auth0ClientFactory.CreateManagementApiClient();

            return await managementApiClient.Users.CreateAsync(new UserCreateRequest()
            {
                Connection = "Username-Password-Authentication",
                Email = loginDto.Login,
                Password = loginDto.Password
            });
        }

        public async Task<AccessTokenResponse> AuthenticateUser(LoginDto loginDto)
        {
            using AuthenticationApiClient authenticationApiClient = _auth0ClientFactory.CreateAuthenticationApiClient();

            var response = await authenticationApiClient.GetTokenAsync(
                               new ResourceOwnerTokenRequest()
                               {
                                   ClientId = _config.ClientId,
                                   ClientSecret = _config.ClientSecret,
                                   Username = loginDto.Login,
                                   Password = loginDto.Password,
                                   Audience = _config.Audience,
                                   Scope = "openid"
                               });
            return response;
        }
    }
}