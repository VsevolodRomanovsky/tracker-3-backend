using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tracker.AuthenticationService.Auth0.ConfigObjects;

namespace Tracker.AuthenticationService.Auth0;

public class Auth0ClientFactory : IAuth0ClientFactory
{
    private readonly IAuthenticationConnection _authenticationConnection;
    private readonly IManagementConnection _managementConnection;
    private readonly Auth0Config _config;

    private AccessTokenResponse? _token;
    private DateTime _tokenExpiresAt = DateTime.MinValue;

    public Auth0ClientFactory(IOptions<Auth0Config> config, 
                              IAuthenticationConnection authenticationConnection, 
                              IManagementConnection managementConnection)
    {
        _authenticationConnection = authenticationConnection;
        _managementConnection = managementConnection;
        _config = config.Value;
    }

    public async Task<ManagementApiClient> CreateManagementApiClient()
    {
        return new ManagementApiClient(await GetAuthTokenAsync(), _config.Domain, _managementConnection);
    }

    public AuthenticationApiClient CreateAuthenticationApiClient()
    {
        return new AuthenticationApiClient(_config.Domain, _authenticationConnection);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        if (IsTokenValid()) 
            return _token.AccessToken;

        await LoadTokenAsync();

        return _token.AccessToken;
    }

    private async Task LoadTokenAsync()
    {
        using AuthenticationApiClient auth0AuthenticationClient = CreateAuthenticationApiClient();
        var tokenRequest = new ClientCredentialsTokenRequest()
        {
            ClientId = _config.ClientId,
            ClientSecret = _config.ClientSecret,
            Audience = $"https://{_config.Domain}/api/v2/"
        };
        var tokenResponse =
            await auth0AuthenticationClient.GetTokenAsync(tokenRequest);

        _token = tokenResponse;
        _tokenExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
    }

    private bool IsTokenValid()
    {
        return _token != null && DateTime.Now <= _tokenExpiresAt;
    }
}