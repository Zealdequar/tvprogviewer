using System.Collections.Generic;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record AddressAttributeModel : BaseTvProgEntityModel
    {
        public AddressAttributeModel()
        {
            Values = new List<AddressAttributeValueModel>();
        }

        public string ControlId { get; set; }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<AddressAttributeValueModel> Values { get; set; }
    }

    public partial record AddressAttributeValueModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}