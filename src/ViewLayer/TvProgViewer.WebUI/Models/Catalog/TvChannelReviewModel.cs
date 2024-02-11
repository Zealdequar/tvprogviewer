using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelReviewOverviewModel : BaseTvProgModel
    {
        public int TvChannelId { get; set; }

        public int RatingSum { get; set; }

        public int TotalReviews { get; set; }
        
        public bool AllowUserReviews { get; set; }

        public bool CanAddNewReview { get; set; }
    }

    public partial record TvChannelReviewsModel : BaseTvProgModel
    {
        public TvChannelReviewsModel()
        {
            Items = new List<TvChannelReviewModel>();
            AddTvChannelReview = new AddTvChannelReviewModel();
            ReviewTypeList = new List<ReviewTypeModel>();
            AddAdditionalTvChannelReviewList = new List<AddTvChannelReviewReviewTypeMappingModel>();
        }

        public int TvChannelId { get; set; }

        public string TvChannelName { get; set; }

        public string TvChannelSeName { get; set; }

        public IList<TvChannelReviewModel> Items { get; set; }

        public AddTvChannelReviewModel AddTvChannelReview { get; set; }

        public IList<ReviewTypeModel> ReviewTypeList { get; set; }

        public IList<AddTvChannelReviewReviewTypeMappingModel> AddAdditionalTvChannelReviewList { get; set; }        
    }

    public partial record ReviewTypeModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public bool VisibleToAllUsers { get; set; }

        public double AverageRating { get; set; }
    }

    public partial record TvChannelReviewModel : BaseTvProgEntityModel
    {
        public TvChannelReviewModel()
        {
            AdditionalTvChannelReviewList = new List<TvChannelReviewReviewTypeMappingModel>();
        }

        public int UserId { get; set; }

        public string UserAvatarUrl { get; set; }

        public string UserName { get; set; }

        public bool AllowViewingProfiles { get; set; }
        
        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public int Rating { get; set; }

        public string WrittenOnStr { get; set; }

        public TvChannelReviewHelpfulnessModel Helpfulness { get; set; }

        public IList<TvChannelReviewReviewTypeMappingModel> AdditionalTvChannelReviewList { get; set; }
    }

    public partial record TvChannelReviewHelpfulnessModel : BaseTvProgModel
    {
        public int TvChannelReviewId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
    }

    public partial record AddTvChannelReviewModel : BaseTvProgModel
    {
        [TvProgResourceDisplayName("Reviews.Fields.Title")]
        public string Title { get; set; }
        
        [TvProgResourceDisplayName("Reviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [TvProgResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentUserLeaveReview { get; set; }

        public bool SuccessfullyAdded { get; set; }

        public bool CanAddNewReview { get; set; }

        public string Result { get; set; }
    }

    public partial record AddTvChannelReviewReviewTypeMappingModel : BaseTvProgEntityModel
    {
        public int TvChannelReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }
    }

    public partial record TvChannelReviewReviewTypeMappingModel : BaseTvProgEntityModel
    {
        public int TvChannelReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }

        public string Name { get; set; }

        public bool VisibleToAllUsers { get; set; }
    }
}