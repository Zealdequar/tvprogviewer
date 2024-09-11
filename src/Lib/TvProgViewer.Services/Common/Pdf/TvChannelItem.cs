using System.Collections.Generic;
using System.ComponentModel;

namespace TvProgViewer.Services.Common.Pdf
{
    /// <summary>
    /// Represents tvChannel entry
    /// </summary>
    public partial class TvChannelItem
    {
        #region Ctor

        public TvChannelItem()
        {
            TvChannelAttributes = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tvChannel name
        /// </summary>
        [DisplayName("Pdf.TvChannel.Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel SKU
        /// </summary>
        [DisplayName("Pdf.TvChannel.Sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets a vendor name
        /// </summary>
        [DisplayName("Pdf.TvChannel.VendorName")]
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel price
        /// </summary>
        [DisplayName("Pdf.TvChannel.Price")]
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel quantity
        /// </summary>
        [DisplayName("Pdf.TvChannel.Quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel total
        /// </summary>
        [DisplayName("Pdf.TvChannel.Total")]
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel attribute description
        /// </summary>
        public List<string> TvChannelAttributes { get; set; }

        #endregion
    }
}