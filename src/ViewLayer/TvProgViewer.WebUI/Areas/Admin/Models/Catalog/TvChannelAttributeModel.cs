using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel attribute model
    /// </summary>
    public partial record TvChannelAttributeModel : BaseTvProgEntityModel, ILocalizedModel<TvChannelAttributeLocalizedModel>
    {
        #region Ctor

        public TvChannelAttributeModel()
        {
            Locales = new List<TvChannelAttributeLocalizedModel>();
            PredefinedTvChannelAttributeValueSearchModel = new PredefinedTvChannelAttributeValueSearchModel();
            TvChannelAttributeTvChannelSearchModel = new TvChannelAttributeTvChannelSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.Fields.Description")]
        public string Description {get;set;}

        public IList<TvChannelAttributeLocalizedModel> Locales { get; set; }

        public PredefinedTvChannelAttributeValueSearchModel PredefinedTvChannelAttributeValueSearchModel { get; set; }

        public TvChannelAttributeTvChannelSearchModel TvChannelAttributeTvChannelSearchModel { get; set; }

        #endregion
    }

    public partial record TvChannelAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.TvChannelAttributes.Fields.Description")]
        public string Description {get;set;}
    }
}