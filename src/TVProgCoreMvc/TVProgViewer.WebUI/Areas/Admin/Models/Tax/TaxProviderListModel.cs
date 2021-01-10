using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax provider list model
    /// </summary>
    public partial record TaxProviderListModel : BasePagedListModel<TaxProviderModel>
    {
    }
}