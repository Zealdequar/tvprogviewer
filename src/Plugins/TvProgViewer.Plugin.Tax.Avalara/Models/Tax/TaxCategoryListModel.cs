using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Tax
{
    /// <summary>
    /// Represents a tax category list model
    /// </summary>
    public record TaxCategoryListModel : BasePagedListModel<TaxCategoryModel>
    {
    }
}