using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record PasswordRecoveryModel : BaseTvProgModel
    {
        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public bool DisplayCaptcha { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

    }
}