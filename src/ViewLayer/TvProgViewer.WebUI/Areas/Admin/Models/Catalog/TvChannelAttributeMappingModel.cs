using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute mapping model
    /// </summary>
    public partial record TvChannelAttributeMappingModel : BaseTvProgEntityModel, ILocalizedModel<TvChannelAttributeMappingLocalizedModel>
    {
        #region Ctor

        public TvChannelAttributeMappingModel()
        {
            AvailableTvChannelAttributes = new List<SelectListItem>();
            Locales = new List<TvChannelAttributeMappingLocalizedModel>();
            ConditionModel = new TvChannelAttributeConditionModel();
            TvChannelAttributeValueSearchModel = new TvChannelAttributeValueSearchModel();
        }

        #endregion

        #region Properties

        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.Attribute")]
        public int TvChannelAttributeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.Attribute")]
        public string TvChannelAttribute { get; set; }

        public IList<SelectListItem> AvailableTvChannelAttributes { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.TextPrompt")]
        public string TextPrompt { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.AttributeControlType")]
        public string AttributeControlType { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        //validation fields
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.MinLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMinLength { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.MaxLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMaxLength { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.FileAllowedExtensions")]
        public string ValidationFileAllowedExtensions { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.FileMaximumSize")]
        [UIHint("Int32Nullable")]
        public int? ValidationFileMaximumSize { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.DefaultValue")]
        public string DefaultValue { get; set; }

        public string ValidationRulesString { get; set; }

        //condition
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Condition")]
        public bool ConditionAllowed { get; set; }

        public string ConditionString { get; set; }

        public TvChannelAttributeConditionModel ConditionModel { get; set; }

        public IList<TvChannelAttributeMappingLocalizedModel> Locales { get; set; }

        public TvChannelAttributeValueSearchModel TvChannelAttributeValueSearchModel { get; set; }

        #endregion
    }

    public partial record TvChannelAttributeMappingLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Fields.TextPrompt")]
        public string TextPrompt { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.DefaultValue")]
        public string DefaultValue { get; set; }
    }
}