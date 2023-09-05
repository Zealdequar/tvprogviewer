using System;
using System.Collections.Generic;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Order
{
    public partial record UserRewardPointsModel : BaseTvProgModel
    {
        public UserRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
        }

        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public PagerModel PagerModel { get; set; }
        public int RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public int MinimumRewardPointsBalance { get; set; }
        public string MinimumRewardPointsAmount { get; set; }

        #region Nested classes

        public partial record RewardPointsHistoryModel : BaseTvProgEntityModel
        {
            [TvProgResourceDisplayName("RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [TvProgResourceDisplayName("RewardPoints.Fields.PointsBalance")]
            public string PointsBalance { get; set; }

            [TvProgResourceDisplayName("RewardPoints.Fields.Message")]
            public string Message { get; set; }

            [TvProgResourceDisplayName("RewardPoints.Fields.CreatedDate")]
            public DateTime CreatedOn { get; set; }

            [TvProgResourceDisplayName("RewardPoints.Fields.EndDate")]
            public DateTime? EndDate { get; set; }
        }

        #endregion
    }
}