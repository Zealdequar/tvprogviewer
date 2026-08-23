using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    /// <summary>
    /// Модель выбора категории ТВ-программы
    /// </summary>
    public partial record TvCategorySelectorModel : BaseTvProgModel
    {
        /// <summary>
        /// Доступные категории (включая обобщённую категорию "Все категории")
        /// </summary>
        public IList<TvCategoryModel> AvailableCategories { get; set; } = new List<TvCategoryModel>();

        /// <summary>
        /// Текущая выбранная категория
        /// </summary>
        public string CurrentCategoryName { get; set; }
    }
}
