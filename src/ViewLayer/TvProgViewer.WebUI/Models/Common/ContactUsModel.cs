using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record ContactUsModel : BaseTvProgModel
    {
        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }
        
        [TvProgResourceDisplayName("ContactUs.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [TvProgResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [TvProgResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}