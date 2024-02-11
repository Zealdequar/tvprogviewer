using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a predefined tvchannel attribute value model
    /// </summary>
    public partial record PredefinedTvChannelAttributeValueModel : BaseTvProgEntityModel, ILocalizedModel<PredefinedTvChannelAttributeValueLocalizedModel>
    {
        #region Ctor

        public PredefinedTvChannelAttributeValueModel()
        {
            Locales = new List<PredefinedTvChannelAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int TvChannelAttributeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.PriceAdjustment")]
        public decimal PriceAdjustment { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.PriceAdjustment")]
        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.PriceAdjustmentUsePercentage")]
        public bool PriceAdjustmentUsePercentage { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.WeightAdjustment")]
        public decimal WeightAdjustment { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.WeightAdjustment")]
        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.Cost")]
        public decimal Cost { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<PredefinedTvChannelAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record PredefinedTvChannelAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.Name")]
        public string Name { get; set; }
    }
}