using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Checkout
{
    /// <summary>
    /// Represents an address validation model
    /// </summary>
    public record AddressValidationModel : BaseTvProgModel
    {
        #region Properties

        public string Message { get; set; }

        public bool IsError { get; set; }

        public bool IsNewAddress { get; set; }

        public int AddressId { get; set; }

        #endregion
    }
}