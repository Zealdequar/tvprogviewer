using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a model for adding or editing a specification attribute
    /// </summary>
    public partial record AddSpecificationAttributeModel : BaseTvProgEntityModel, ILocalizedModel<AddSpecificationAttributeLocalizedModel>
    {
        #region Ctor

        public AddSpecificationAttributeModel()
        {
            AvailableOptions = new List<SelectListItem>();
            AvailableAttributes = new List<SelectListItem>();
            ShowOnTvChannelPage = true;
            AttributeName = string.Empty;
            AttributeTypeName = string.Empty;
            Value = string.Empty;
            ValueRaw = string.Empty;
            Locales = new List<AddSpecificationAttributeLocalizedModel>();
        }

        #endregion

        #region Properties

        public int SpecificationId { get; set; }

        public int AttributeTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.AttributeType")]
        public string AttributeTypeName { get; set; }

        public int AttributeId { get; set; }

        public int TvChannelId { get; set; }

        public IList<SelectListItem> AvailableAttributes { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.SpecificationAttribute")]
        public string AttributeName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.CustomValue")]
        public string ValueRaw { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.CustomValue")]
        public string Value { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.AllowFiltering")]
        public bool AllowFiltering { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.ShowOnTvChannelPage")]
        public bool ShowOnTvChannelPage { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.SpecificationAttributes.Fields.SpecificationAttributeOption")]
        public int SpecificationAttributeOptionId { get; set; }

        public IList<SelectListItem> AvailableOptions { get; set; }

        public IList<AddSpecificationAttributeLocalizedModel> Locales { get; set; }

        #endregion
    }
}