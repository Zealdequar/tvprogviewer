using System;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Log
{
    /// <summary>
    /// Represents a tax transaction log model
    /// </summary>
    public record TaxTransactionLogModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.StatusCode")]
        public int StatusCode { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.Url")]
        public string Url { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.RequestMessage")]
        public string RequestMessage { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.ResponseMessage")]
        public string ResponseMessage { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.User")]
        public int? UserId { get; set; }
        public string UserEmail { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        #endregion
    }
}