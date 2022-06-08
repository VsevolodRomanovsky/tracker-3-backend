using Tracker.AuthenticationService.Data.Models;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public interface IDbService
    {
        public Task<WebSecurityUsersExtended> GetWebSecurityUsersExtended(LoginDto loginDto);

        public Task UpdateWebSecurityUsersExtended(LoginDto loginDto);
    }
}
