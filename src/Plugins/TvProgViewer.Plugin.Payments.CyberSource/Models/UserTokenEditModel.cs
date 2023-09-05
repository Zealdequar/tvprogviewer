using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.CyberSource.Models
{
    /// <summary>
    /// Represents a CyberSource user token edit model
    /// </summary>
    public record UserTokenEditModel : BaseTvProgModel
    {
        #region Ctor

        public UserTokenEditModel()
        {
            UserToken = new UserTokenModel();
        }

        #endregion

        #region Properties

        public UserTokenModel UserToken { get; set; }

        #endregion
    }
}