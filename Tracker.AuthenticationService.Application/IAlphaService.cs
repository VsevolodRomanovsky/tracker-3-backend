using Newtonsoft.Json.Linq;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public interface IAlphaService
    {
        public Task<JObject> GetUserDataFromAlpha(LoginDto loginDto);
    }
}
