using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute value model
    /// </summary>
    public partial record TvChannelAttributeValueModel : BaseTvProgEntityModel, ILocalizedModel<TvChannelAttributeValueLocalizedModel>
    {
        #region Ctor

        public TvChannelAttributeValueModel()
        {
            TvChannelPictureModels = new List<TvChannelPictureModel>();
            Locales = new List<TvChannelAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int TvChannelAttributeMappingId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AttributeValueType")]
        public int AttributeValueTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AttributeValueType")]
        public string AttributeValueTypeName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel")]
        public int AssociatedTvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel")]
        public string AssociatedTvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.ColorSquaresRgb")]
        public string ColorSquaresRgb { get; set; }

        public bool DisplayColorSquaresRgb { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.ImageSquaresPicture")]
        [UIHint("Picture")]
        public int ImageSquaresPictureId { get; set; }

        public bool DisplayImageSquaresPicture { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.PriceAdjustment")]
        public decimal PriceAdjustment { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.PriceAdjustment")]
        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.PriceAdjustmentUsePercentage")]
        public bool PriceAdjustmentUsePercentage { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.WeightAdjustment")]
        public decimal WeightAdjustment { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.WeightAdjustment")]
        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Cost")]
        public decimal Cost { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.UserEntersQty")]
        public bool UserEntersQty { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Quantity")]
        public int Quantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Picture")]
        public int PictureId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Picture")]
        public string PictureThumbnailUrl { get; set; }

        public IList<TvChannelPictureModel> TvChannelPictureModels { get; set; }

        public IList<TvChannelAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record TvChannelAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}