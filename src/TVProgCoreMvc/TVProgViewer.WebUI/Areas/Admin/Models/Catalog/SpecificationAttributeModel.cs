using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute model
    /// </summary>
    public partial record SpecificationAttributeModel : BaseTvProgEntityModel, ILocalizedModel<SpecificationAttributeLocalizedModel>
    {
        #region Ctor

        public SpecificationAttributeModel()
        {
            Locales = new List<SpecificationAttributeLocalizedModel>();
            SpecificationAttributeOptionSearchModel = new SpecificationAttributeOptionSearchModel();
            SpecificationAttributeProductSearchModel = new SpecificationAttributeProductSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<SpecificationAttributeLocalizedModel> Locales { get; set; }

        public SpecificationAttributeOptionSearchModel SpecificationAttributeOptionSearchModel { get; set; }

        public SpecificationAttributeProductSearchModel SpecificationAttributeProductSearchModel { get; set; }

        #endregion
    }
}