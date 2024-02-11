using System.Collections.Generic;
using System.ComponentModel;

namespace TvProgViewer.Services.Common.Pdf
{
    /// <summary>
    /// Represents a PDF catalog entry
    /// </summary>
    public partial class CatalogItem : TvChannelItem
    {
        #region Ctor

        public CatalogItem()
        {
            PicturePaths = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entry description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        [DisplayName("Pdf.TvChannel.StockQuantity")]
        public string Stock { get; set; }

        /// <summary>
        /// Gets or sets the entry weight
        /// </summary>
        [DisplayName("Pdf.TvChannel.Weight")]
        public string Weight { get; set; }

        /// <summary>
        /// Gets or sets a set of paths to entry images
        /// </summary>
        public HashSet<string> PicturePaths { get; set; }

        #endregion
    }
}