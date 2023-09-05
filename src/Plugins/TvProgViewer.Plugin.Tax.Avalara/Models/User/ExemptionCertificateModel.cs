using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.User
{
    /// <summary>
    /// Represents a tax exemption certificate model
    /// </summary>
    public record ExemptionCertificateModel : BaseTvProgEntityModel
    {
        #region Properties

        public string Status { get; set; }

        public string SignedDate { get; set; }

        public string ExpirationDate { get; set; }

        public string ExposureZone { get; set; }

        #endregion
    }
}