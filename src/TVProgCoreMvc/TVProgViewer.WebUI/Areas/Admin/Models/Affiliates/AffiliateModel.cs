using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliate model
    /// </summary>
    public partial record AffiliateModel : BaseTvProgEntityModel
    {
        #region Ctor

        public AffiliateModel()
        {
            Address = new AddressModel();
            AffiliatedOrderSearchModel= new AffiliatedOrderSearchModel();
            AffiliatedUserSearchModel = new AffiliatedUserSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }
        
        [TvProgResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [TvProgResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        public string FriendlyUrlName { get; set; }
        
        [TvProgResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        public AddressModel Address { get; set; }

        public AffiliatedOrderSearchModel AffiliatedOrderSearchModel { get; set; }

        public AffiliatedUserSearchModel AffiliatedUserSearchModel { get; set; }

        #endregion
    }
}