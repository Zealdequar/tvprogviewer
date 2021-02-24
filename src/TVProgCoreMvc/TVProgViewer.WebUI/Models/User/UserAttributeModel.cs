using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.User
{
    public partial record UserAttributeModel : BaseTvProgEntityModel
    {
        public UserAttributeModel()
        {
            Values = new List<UserAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<UserAttributeValueModel> Values { get; set; }

    }

    public partial record UserAttributeValueModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}