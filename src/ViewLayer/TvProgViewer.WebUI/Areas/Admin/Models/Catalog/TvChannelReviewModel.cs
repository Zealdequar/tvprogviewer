using System;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel review model
    /// </summary>
    public partial record TvChannelReviewModel : BaseTvProgEntityModel
    {
        #region Ctor

        public TvChannelReviewModel()
        {
            TvChannelReviewReviewTypeMappingSearchModel = new TvChannelReviewReviewTypeMappingSearchModel();            
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.Store")]
        public string StoreName { get; set; }
        public bool ShowStoreName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.TvChannel")]
        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.User")]
        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.User")]
        public string UserInfo { get; set; }
        
        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.Title")]
        public string Title { get; set; }
        
        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.ReviewText")]
        public string ReviewText { get; set; }
        
        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.ReplyText")]
        public string ReplyText { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.Rating")]
        public int Rating { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviews.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        public TvChannelReviewReviewTypeMappingSearchModel TvChannelReviewReviewTypeMappingSearchModel { get; set; }

        #endregion
    }
}