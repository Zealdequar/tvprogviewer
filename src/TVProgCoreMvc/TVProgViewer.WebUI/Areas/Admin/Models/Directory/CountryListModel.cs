using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a country list model
    /// </summary>
    public partial record CountryListModel : BasePagedListModel<CountryModel>
    {
    }
}