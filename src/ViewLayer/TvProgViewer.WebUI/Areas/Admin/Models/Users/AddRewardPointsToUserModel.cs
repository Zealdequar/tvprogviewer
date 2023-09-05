using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a reward points model to add to the user
    /// </summary>
    public partial record AddRewardPointsToUserModel : BaseTvProgModel
    {
        #region Ctor

        public AddRewardPointsToUserModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Points")]
        public int Points { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Message")]
        public string Message { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Store")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.ActivatePointsImmediately")]
        public bool ActivatePointsImmediately { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.ActivationDelay")]
        public int ActivationDelay { get; set; }

        public int ActivationDelayPeriodId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.PointsValidity")]
        [UIHint("Int32Nullable")]
        public int? PointsValidity { get; set; }

        #endregion
    }
}