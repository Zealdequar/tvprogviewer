using System.ComponentModel.DataAnnotations;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Newsletter
{
    public partial record NewsletterBoxModel : BaseTvProgModel
    {
        [DataType(DataType.EmailAddress)]
        public string NewsletterEmail { get; set; }
        public bool AllowToUnsubscribe { get; set; }
    }
}