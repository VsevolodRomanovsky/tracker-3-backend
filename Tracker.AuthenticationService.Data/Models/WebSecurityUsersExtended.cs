using System.ComponentModel.DataAnnotations;

namespace Tracker.AuthenticationService.Data.Models
{
    public partial class WebSecurityUsersExtended
    {
        public string Userid { get; set; }
        public bool HasNewAccount { get; set; }
    }
}
