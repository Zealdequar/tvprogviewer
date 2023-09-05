using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Log
{
    /// <summary>
    /// Represents a tax transaction log list model
    /// </summary>
    public record TaxTransactionLogListModel : BasePagedListModel<TaxTransactionLogModel>
    {
    }
}