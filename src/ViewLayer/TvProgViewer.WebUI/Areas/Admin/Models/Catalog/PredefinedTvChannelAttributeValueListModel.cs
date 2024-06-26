﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a predefined tvchannel attribute value list model
    /// </summary>
    public partial record PredefinedTvChannelAttributeValueListModel : BasePagedListModel<PredefinedTvChannelAttributeValueModel>
    {
    }
}