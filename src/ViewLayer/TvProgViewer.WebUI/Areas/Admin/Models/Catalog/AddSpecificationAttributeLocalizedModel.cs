using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an add specification attribute localized model
    /// </summary>
    public partial record AddSpecificationAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.CustomValue")]
        public string ValueRaw { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.CustomValue")]
        public string Value { get; set; }
    }
}