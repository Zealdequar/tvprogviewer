using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a stock quantity history search model
    /// </summary>
    public partial record StockQuantityHistorySearchModel : BaseSearchModel
    {
        #region Ctor

        public StockQuantityHistorySearchModel()
        {
            AvailableWarehouses = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.List.SearchWarehouse")]
        public int WarehouseId { get; set; }

        public IList<SelectListItem> AvailableWarehouses { get; set; }

        #endregion
    }
}