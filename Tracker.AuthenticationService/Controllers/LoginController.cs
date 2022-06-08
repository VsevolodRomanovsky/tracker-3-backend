using Auth0.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Tracker.AuthenticationService.Dto;
using Tracker.AuthenticationService.Application;
using Newtonsoft.Json.Linq;

namespace Tracker.AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAlphaService _alphaService;
        private readonly IDbService _dbService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, IAuthenticationService authenticationService, IAlphaService alphaService, IDbService dbService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _alphaService = alphaService;
            _dbService = dbService;
        }

        // POST api/<LoginController>
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var dRes = await _dbService.GetWebSecurityUsersExtended(loginDto);
            if (dRes == null) 
                return Forbid();

            if(dRes.HasNewAccount)
            {
                try
                {
                    var authenticationResult = await _authenticationService.AuthenticateUser(loginDto);
                    return Ok(authenticationResult);
                }
                catch (ErrorApiException e)
                {
                    return Unauthorized(e.Message);
                }
            }
            else
            {
                var alphaResult = await _alphaService.GetUserDataFromAlpha(loginDto);
                var res = alphaResult["message"]?.Value<string>();
                if (res == "Success")
                {
                    await _authenticationService.CreateUser(loginDto);
                    await _dbService.UpdateWebSecurityUsersExtended(loginDto);
                    var authenticationResult = await _authenticationService.AuthenticateUser(loginDto);
                    return Ok(authenticationResult);
                }
                return Unauthorized(alphaResult);
            }
        }
    }
}
