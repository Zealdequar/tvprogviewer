using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record ChangePasswordModel : BaseTvProgModel
    {
        [DataType(DataType.Password)]
        [NoTrim]
        [TvProgResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [TvProgResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [TvProgResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}