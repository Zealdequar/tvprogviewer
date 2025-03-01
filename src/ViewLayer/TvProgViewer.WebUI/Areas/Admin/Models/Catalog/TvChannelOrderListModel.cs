﻿using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel order list model
    /// </summary>
    public partial record TvChannelOrderListModel : BasePagedListModel<OrderModel>
    {
    }
}