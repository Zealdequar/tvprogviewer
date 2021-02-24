﻿using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock product list model
    /// </summary>
    public partial record LowStockProductListModel : BasePagedListModel<LowStockProductModel>
    {
    }
}