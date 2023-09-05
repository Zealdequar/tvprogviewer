using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record ExternalAuthenticationMethodModel : BaseTvProgModel
    {
        public Type ViewComponent { get; set; }
    }
}