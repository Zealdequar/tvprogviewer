using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute model
    /// </summary>
    public partial record UserAttributeModel : BaseTvProgEntityModel, ILocalizedModel<UserAttributeLocalizedModel>
    {
        #region Ctor

        public UserAttributeModel()
        {
            Locales = new List<UserAttributeLocalizedModel>();
            UserAttributeValueSearchModel = new UserAttributeValueSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
        public string AttributeControlTypeName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<UserAttributeLocalizedModel> Locales { get; set; }

        public UserAttributeValueSearchModel UserAttributeValueSearchModel { get; set; }

        #endregion
    }

    public partial record UserAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        public string Name { get; set; }
    }
}