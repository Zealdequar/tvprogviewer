﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a never sold tvChannels report list model
    /// </summary>
    public partial record NeverSoldReportListModel : BasePagedListModel<NeverSoldReportModel>
    {
    }
}