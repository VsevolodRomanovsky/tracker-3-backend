using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi.Models;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public interface IAuthenticationService
    {
        public Task<User> CreateUser(LoginDto loginDto);
        public Task<AccessTokenResponse> AuthenticateUser(LoginDto loginDto);
    }
}
