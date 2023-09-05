using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a country list model
    /// </summary>
    public partial record CountryListModel : BasePagedListModel<CountryModel>
    {
    }
}