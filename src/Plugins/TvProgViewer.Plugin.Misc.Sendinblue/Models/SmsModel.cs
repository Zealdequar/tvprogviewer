using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents SMS model
    /// </summary>
    public record SmsModel : BaseTvProgEntityModel
    {
        #region Ctor

        public SmsModel()
        {
            AvailableMessages = new List<SelectListItem>();
            AvailableSmartPhoneTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        public int MessageId { get; set; }

        public IList<SelectListItem> AvailableMessages { get; set; }

        public string DefaultSelectedMessageId { get; set; }

        public string Name { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Sendinblue.SmartPhoneType")]
        public int SmartPhoneTypeId { get; set; }

        public IList<SelectListItem> AvailableSmartPhoneTypes { get; set; }

        public string DefaultSelectedSmartPhoneTypeId { get; set; }

        public string SmartPhoneType { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Sendinblue.SMSText")]
        public string Text { get; set; }

        #endregion
    }
}