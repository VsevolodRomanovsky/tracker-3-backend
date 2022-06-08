using Auth0.AuthenticationApi;
using Auth0.ManagementApi;

namespace Tracker.AuthenticationService.Auth0;

public interface IAuth0ClientFactory
{
    Task<ManagementApiClient> CreateManagementApiClient();
    AuthenticationApiClient CreateAuthenticationApiClient();
}