using System.ComponentModel.DataAnnotations;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record LoginModel : BaseTvProgModel
    {
        public bool CheckoutAsGuest { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Account.Login.Fields.Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }

        public UserRegistrationType RegistrationType { get; set; }

        [TvProgResourceDisplayName("Account.Login.Fields.Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [TvProgResourceDisplayName("Account.Login.Fields.Password")]
        public string Password { get; set; }

        [TvProgResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}