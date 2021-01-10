using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Vendors
{
    public partial record VendorAttributeModel : BaseTvProgEntityModel
    {
        public VendorAttributeModel()
        {
            Values = new List<VendorAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<VendorAttributeValueModel> Values { get; set; }

    }

    public partial record VendorAttributeValueModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}