﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a manufacturer model to add to the discount
    /// </summary>
    public partial record AddManufacturerToDiscountModel : BaseTvProgModel
    {
        #region Ctor

        public AddManufacturerToDiscountModel()
        {
            SelectedManufacturerIds = new List<int>();
        }
        #endregion

        #region Properties

        public int DiscountId { get; set; }

        public IList<int> SelectedManufacturerIds { get; set; }

        #endregion
    }
}