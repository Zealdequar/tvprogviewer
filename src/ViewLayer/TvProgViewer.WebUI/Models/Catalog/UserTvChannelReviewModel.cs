using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record UserTvChannelReviewModel : BaseTvProgModel
    {
        public UserTvChannelReviewModel()
        {
            AdditionalTvChannelReviewList = new List<TvChannelReviewReviewTypeMappingModel>();
        }
        public int TvChannelId { get; set; }
        public string TvChannelName { get; set; }
        public string TvChannelSeName { get; set; }
        public string Title { get; set; }
        public string ReviewText { get; set; }
        public string ReplyText { get; set; }
        public int Rating { get; set; }
        public string WrittenOnStr { get; set; }
        public string ApprovalStatus { get; set; }
        public IList<TvChannelReviewReviewTypeMappingModel> AdditionalTvChannelReviewList { get; set; }
    }

    public partial record UserTvChannelReviewsModel : BaseTvProgModel
    {
        public UserTvChannelReviewsModel()
        {
            TvChannelReviews = new List<UserTvChannelReviewModel>();
        }

        public IList<UserTvChannelReviewModel> TvChannelReviews { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested class

        /// <summary>
        /// record that has only page for route value. Used for (My Account) My TvChannel Reviews pagination
        /// </summary>
        public partial record UserTvChannelReviewsRouteValues : IRouteValues
        {
            public int PageNumber { get; set; }
        }

        #endregion
    }
}