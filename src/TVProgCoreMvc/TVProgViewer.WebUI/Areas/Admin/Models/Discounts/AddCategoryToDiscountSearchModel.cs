using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a category search model to add to the discount
    /// </summary>
    public partial record AddCategoryToDiscountSearchModel : BaseSearchModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        #endregion
    }
}