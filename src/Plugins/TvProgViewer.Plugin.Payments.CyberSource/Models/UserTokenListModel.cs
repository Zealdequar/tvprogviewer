using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a CyberSource user token list model
    /// </summary>
    public record UserTokenListModel : BaseTvProgModel
    {
        #region Ctor

        public UserTokenListModel()
        {
            Tokens = new List<UserTokenDetailsModel>();
        }

        #endregion

        #region Properties

        public IList<UserTokenDetailsModel> Tokens { get; set; }

        #endregion

        #region Nested classes

        public record UserTokenDetailsModel : BaseTvProgEntityModel
        {
            public string CardNumber { get; set; }

            public string ThreeDigitCardType { get; set; }

            public string CardExpirationYear { get; set; }

            public string CardExpirationMonth { get; set; }
        }

        #endregion
    }
}