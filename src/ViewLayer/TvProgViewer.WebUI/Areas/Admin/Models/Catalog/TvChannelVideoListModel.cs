﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel video list model
    /// </summary>
    public partial record TvChannelVideoListModel : BasePagedListModel<TvChannelVideoModel>
    {
    }
}