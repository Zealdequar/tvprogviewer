using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a currency list model
    /// </summary>
    public partial record CurrencyListModel : BasePagedListModel<CurrencyModel>
    {
    }
}