using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record PasswordRecoveryConfirmModel : BaseTvProgModel
    {
        [NoTrim]
        [DataType(DataType.Password)]
        [TvProgResourceDisplayName("Account.PasswordRecovery.NewPassword")]
        public string NewPassword { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        [TvProgResourceDisplayName("Account.PasswordRecovery.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }

        public string ReturnUrl { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
    }
}