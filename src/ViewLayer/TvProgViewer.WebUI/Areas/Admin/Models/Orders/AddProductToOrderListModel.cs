﻿using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a product list model to add to the order
    /// </summary>
    public partial record AddProductToOrderListModel : BasePagedListModel<ProductModel>
    {
    }
}