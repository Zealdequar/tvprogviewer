using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a CyberSource user token model
    /// </summary>
    public record UserTokenModel : BaseTvProgEntityModel
    {
        #region Ctor

        public UserTokenModel()
        {
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Payment.CardNumber")]
        public string CardNumber { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireMonth { get; set; }

        [TvProgResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireYear { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }

        public IList<SelectListItem> ExpireYears { get; set; }

        #endregion
    }
}