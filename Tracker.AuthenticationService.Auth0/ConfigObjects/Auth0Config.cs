using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.AuthenticationService.Auth0.ConfigObjects
{
    public class Auth0Config
    {
        public const string Auth0 = "Auth0";
        /// <summary>
        /// The authority URL taken from the setup for your application
        /// in Auth0.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// The Audience specified in your Auth0 Configuration.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// The client id specified in your Auth0 Configuration.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The client secret specified in your Auth0 Configuration.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The client domain  specified in your Auth0 Configuration.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Alpha service end point.
        /// </summary>
        public string AlphaUrl { get; set; }
    }
}
