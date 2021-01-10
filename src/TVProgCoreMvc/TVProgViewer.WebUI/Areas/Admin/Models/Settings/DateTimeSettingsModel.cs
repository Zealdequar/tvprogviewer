using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a date time settings model
    /// </summary>
    public partial record DateTimeSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Ctor

        public DateTimeSettingsModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToSetTimeZone")]
        public bool AllowUsersToSetTimeZone { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultStoreTimeZone")]
        public string DefaultStoreTimeZoneId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultStoreTimeZone")]
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        #endregion
    }
}