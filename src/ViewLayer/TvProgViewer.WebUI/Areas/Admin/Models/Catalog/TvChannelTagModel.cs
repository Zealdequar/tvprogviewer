using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel tag model
    /// </summary>
    public partial record TvChannelTagModel : BaseTvProgEntityModel, ILocalizedModel<TvChannelTagLocalizedModel>
    {
        #region Ctor

        public TvChannelTagModel()
        {
            Locales = new List<TvChannelTagLocalizedModel>();
        }
        
        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelTags.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelTags.Fields.TvChannelCount")]
        public int TvChannelCount { get; set; }

        public IList<TvChannelTagLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record TvChannelTagLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelTags.Fields.Name")]
        public string Name { get; set; }
    }
}