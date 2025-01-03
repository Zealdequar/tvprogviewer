﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a GDPR consent list model
    /// </summary>
    public partial record GdprConsentListModel : BasePagedListModel<GdprConsentModel>
    {
    }
}