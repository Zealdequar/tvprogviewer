﻿using Microsoft.AspNetCore.Http;

namespace TvProgViewer.Services.Authentication
{
    /// <summary>
    /// Represents default values related to authentication services
    /// </summary>
    public static partial class TvProgAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for authentication scheme
        /// </summary>
        public static string AuthenticationScheme => "Authentication";

        /// <summary>
        /// The default value used for external authentication scheme
        /// </summary>
        public static string ExternalAuthenticationScheme => "ExternalAuthentication";

        /// <summary>
        /// The issuer that should be used for any claims that are created
        /// </summary>
        public static string ClaimsIssuer => "tvProgViewer";

        /// <summary>
        /// The default value for the login path
        /// </summary>
        public static PathString LoginPath => new("/login");
        
        /// <summary>
        /// The default value for the access denied path
        /// </summary>
        public static PathString AccessDeniedPath => new("/page-not-found");

        /// <summary>
        /// The default value of the return URL parameter
        /// </summary>
        public static string ReturnUrlParameter => string.Empty;

        /// <summary>
        /// Gets a key to store external authentication errors to session
        /// </summary>
        public static string ExternalAuthenticationErrorsSessionKey => "nop.externalauth.errors";
    }
}