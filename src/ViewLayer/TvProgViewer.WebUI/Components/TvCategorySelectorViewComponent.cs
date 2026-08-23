using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.WebUI.Factories;

namespace TvProgViewer.WebUI.Components
{
    /// <summary>
    /// Компонент выбора категории ТВ-программы
    /// </summary>
    public sealed class TvCategorySelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvCategorySelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        /// <summary>
        /// Отображает список категорий ТВ-программы
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _commonModelFactory.PrepareTvCategorySelectorModelAsync());
        }
    }
}
