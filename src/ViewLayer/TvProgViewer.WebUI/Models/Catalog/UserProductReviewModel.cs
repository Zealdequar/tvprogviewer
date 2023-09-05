using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record UserProductReviewModel : BaseTvProgModel
    {
        public UserProductReviewModel()
        {
            AdditionalProductReviewList = new List<ProductReviewReviewTypeMappingModel>();
        }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public string Title { get; set; }
        public string ReviewText { get; set; }
        public string ReplyText { get; set; }
        public int Rating { get; set; }
        public string WrittenOnStr { get; set; }
        public string ApprovalStatus { get; set; }
        public IList<ProductReviewReviewTypeMappingModel> AdditionalProductReviewList { get; set; }
    }

    public partial record UserProductReviewsModel : BaseTvProgModel
    {
        public UserProductReviewsModel()
        {
            ProductReviews = new List<UserProductReviewModel>();
        }

        public IList<UserProductReviewModel> ProductReviews { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested class

        /// <summary>
        /// record that has only page for route value. Used for (My Account) My Product Reviews pagination
        /// </summary>
        public partial record UserProductReviewsRouteValues : IRouteValues
        {
            public int PageNumber { get; set; }
        }

        #endregion
    }
}