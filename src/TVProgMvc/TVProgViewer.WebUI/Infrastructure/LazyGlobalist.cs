using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;

namespace TVProgViewer.WebUI.Infrastructure
{
    public sealed class LazyGlobalist
    {
        private static readonly Lazy<LazyGlobalist> _instance = new Lazy<LazyGlobalist>(() => new LazyGlobalist());

        public LazyGlobalist(){}

        public static LazyGlobalist Instance { get { return _instance.Value; } }

        public long? UserId(HttpContext httpContext)
        {
            long userId;
            if (httpContext.Session["ClientCode"] == null)
            {
                var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    try
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        if (authTicket != null && !authTicket.Expired)
                        {
                            var userData = authTicket.UserData.Split(',');
                            httpContext.Session["ClientCode"] = userData[1];
                        }
                    }
                    catch (CryptographicException cex)
                    {
                        FormsAuthentication.SignOut();
                        return null;
                    }
                }
            }
            if (long.TryParse(httpContext.Session["ClientCode"]?.ToString(), out userId))
              return userId;

            return null;
        }
    }
}