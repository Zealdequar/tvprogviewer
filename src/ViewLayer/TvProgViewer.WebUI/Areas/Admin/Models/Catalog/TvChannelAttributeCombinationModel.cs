using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute combination model
    /// </summary>
    public partial record TvChannelAttributeCombinationModel : BaseTvProgEntityModel
    {
        #region Ctor

        public TvChannelAttributeCombinationModel()
        {
            TvChannelAttributes = new List<TvChannelAttributeModel>();
            TvChannelPictureModels = new List<TvChannelPictureModel>();
            Warnings = new List<string>();
        }

        #endregion

        #region Properties

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Attributes")]
        public string AttributesXml { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.AllowOutOfStockOrders")]
        public bool AllowOutOfStockOrders { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Sku")]
        public string Sku { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.ManufacturerPartNumber")]
        public string ManufacturerPartNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Gtin")]
        public string Gtin { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.OverriddenPrice")]
        [UIHint("DecimalNullable")]
        public decimal? OverriddenPrice { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.NotifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Picture")]
        public int PictureId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.MinStockQuantity")]
        public int MinStockQuantity { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public IList<TvChannelAttributeModel> TvChannelAttributes { get; set; }

        public IList<TvChannelPictureModel> TvChannelPictureModels { get; set; }

        public IList<string> Warnings { get; set; }

        #endregion

        #region Nested classes

        public partial record TvChannelAttributeModel : BaseTvProgEntityModel
        {
            public TvChannelAttributeModel()
            {
                Values = new List<TvChannelAttributeValueModel>();
            }

            public int TvChannelAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<TvChannelAttributeValueModel> Values { get; set; }
        }

        public partial record TvChannelAttributeValueModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }

            public string Checked { get; set; }
        }

        #endregion
    }
}