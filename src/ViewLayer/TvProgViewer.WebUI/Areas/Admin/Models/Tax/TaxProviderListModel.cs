using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax provider list model
    /// </summary>
    public partial record TaxProviderListModel : BasePagedListModel<TaxProviderModel>
    {
    }
}