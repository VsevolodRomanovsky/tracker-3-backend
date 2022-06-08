using System.Security.Claims;

namespace Tracker.AuthenticationService.Auth0.Abstractions
{
    internal interface IPermissionValidator
    {
        bool CurrentUserHasPermission(ClaimsIdentity user, string permission);
    }
}
