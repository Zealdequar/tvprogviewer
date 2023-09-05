using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Stores
{
    /// <summary>
    /// Represents a store model
    /// </summary>
    public partial record StoreModel : BaseTvProgEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        #region Ctor

        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
            AvailableLanguages = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        public string Url { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        public string Hosts { get; set; }

        //default language
        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultLanguage")]
        public int DefaultLanguageId { get; set; }

        public IList<SelectListItem> AvailableLanguages { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyName")]
        public string CompanyName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyAddress")]
        public string CompanyAddress { get; set; }

        [DataType(DataType.PhoneNumber)]
        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyPhoneNumber")]
        public string CompanyPhoneNumber { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyVat")]
        public string CompanyVat { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultMetaKeywords")]
        public string DefaultMetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultMetaDescription")]
        public string DefaultMetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultTitle")]
        public string DefaultTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.HomepageTitle")]
        public string HomepageTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.HomepageDescription")]
        public string HomepageDescription { get; set; }

        public IList<StoreLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record StoreLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultMetaKeywords")]
        public string DefaultMetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultMetaDescription")]
        public string DefaultMetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultTitle")]
        public string DefaultTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.HomepageTitle")]
        public string HomepageTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Stores.Fields.HomepageDescription")]
        public string HomepageDescription { get; set; }
    }
}