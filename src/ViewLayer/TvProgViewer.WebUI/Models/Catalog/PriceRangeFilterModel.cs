﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannels price range filter model
    /// </summary>
    public partial record PriceRangeFilterModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the selected price range
        /// </summary>
        public PriceRangeModel SelectedPriceRange { get; set; }

        /// <summary>
        /// Gets or sets the available price range
        /// </summary>
        public PriceRangeModel AvailablePriceRange { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public PriceRangeFilterModel()
        {
            SelectedPriceRange = new PriceRangeModel();
            AvailablePriceRange = new PriceRangeModel();
        }

        #endregion
    }
}
