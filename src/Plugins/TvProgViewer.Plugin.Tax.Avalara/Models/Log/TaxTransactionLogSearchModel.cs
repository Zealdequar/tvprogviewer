using System;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Log
{
    /// <summary>
    /// Represents a tax transaction log search model
    /// </summary>
    public record TaxTransactionLogSearchModel : BaseSearchModel
    {
        #region Properties

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.Search.CreatedFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedFrom { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Log.Search.CreatedTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedTo { get; set; }

        #endregion
    }
}