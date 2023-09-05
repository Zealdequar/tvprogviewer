using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Vendors
{
    public partial record VendorInfoModel : BaseTvProgModel
    {
        public VendorInfoModel()
        {
            VendorAttributes = new List<VendorAttributeModel>();
        }

        [TvProgResourceDisplayName("Account.VendorInfo.Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("Account.VendorInfo.Email")]
        public string Email { get; set; }

        [TvProgResourceDisplayName("Account.VendorInfo.Description")]
        public string Description { get; set; }

        [TvProgResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set; }
    }
}