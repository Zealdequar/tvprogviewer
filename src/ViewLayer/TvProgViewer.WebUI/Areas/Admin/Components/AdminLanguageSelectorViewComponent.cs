using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the admin language selector
    /// </summary>
    public partial class AdminLanguageSelectorViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;

        #endregion

        #region Ctor

        public AdminLanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //prepare model
            var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

            return View(model);
        }

        #endregion
    }
}