using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute group model
    /// </summary>
    public partial record SpecificationAttributeGroupModel : BaseTvProgEntityModel, ILocalizedModel<SpecificationAttributeGroupLocalizedModel>
    {
        #region Ctor

        public SpecificationAttributeGroupModel()
        {
            Locales = new List<SpecificationAttributeGroupLocalizedModel>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<SpecificationAttributeGroupLocalizedModel> Locales { get; set; }

        #endregion
    }
}
