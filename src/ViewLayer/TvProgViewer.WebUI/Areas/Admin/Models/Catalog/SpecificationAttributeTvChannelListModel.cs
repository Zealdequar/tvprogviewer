﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a list model of tvChannels that use the specification attribute
    /// </summary>
    public partial record SpecificationAttributeTvChannelListModel : BasePagedListModel<SpecificationAttributeTvChannelModel>
    {
    }
}