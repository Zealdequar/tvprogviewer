using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value model
    /// </summary>
    public partial record UserAttributeValueModel : BaseTvProgEntityModel, ILocalizedModel<UserAttributeValueLocalizedModel>
    {
        #region Ctor

        public UserAttributeValueModel()
        {
            Locales = new List<UserAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int UserAttributeId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<UserAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record UserAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}