using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record MultiFactorAuthenticationModel : BaseTvProgModel
    {
        public MultiFactorAuthenticationModel()
        {
            Providers = new List<MultiFactorAuthenticationProviderModel>();
        }

        [TvProgResourceDisplayName("Account.MultiFactorAuthentication.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

        public List<MultiFactorAuthenticationProviderModel> Providers { get; set; }

        public string Message { get; set; }
        
    }
}
