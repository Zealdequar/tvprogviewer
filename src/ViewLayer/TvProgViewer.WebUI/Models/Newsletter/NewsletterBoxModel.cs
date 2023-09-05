using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Newsletter
{
    public partial record NewsletterBoxModel : BaseTvProgModel
    {
        [DataType(DataType.EmailAddress)]
        public string NewsletterEmail { get; set; }
        public bool AllowToUnsubscribe { get; set; }
    }
}