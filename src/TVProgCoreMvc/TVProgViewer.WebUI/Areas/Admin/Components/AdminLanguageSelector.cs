using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Areas.Admin.Factories;

namespace TVProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the admin language selector
    /// </summary>
    public class AdminLanguageSelectorViewComponent : TvProgViewComponent
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
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //prepare model
            var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

            return View(model);
        }

        #endregion
    }
}