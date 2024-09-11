using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a tvChannel availability range model
    /// </summary>
    public partial record TvChannelAvailabilityRangeModel : BaseTvProgEntityModel, ILocalizedModel<TvChannelAvailabilityRangeLocalizedModel>
    {
        #region Ctor

        public TvChannelAvailabilityRangeModel()
        {
            Locales = new List<TvChannelAvailabilityRangeLocalizedModel>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.TvChannelAvailabilityRanges.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.TvChannelAvailabilityRanges.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<TvChannelAvailabilityRangeLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record TvChannelAvailabilityRangeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.TvChannelAvailabilityRanges.Fields.Name")]
        public string Name { get; set; }
    }
}