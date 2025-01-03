﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute value list model
    /// </summary>
    public partial record TvChannelAttributeValueListModel : BasePagedListModel<TvChannelAttributeValueModel>
    {
    }
}