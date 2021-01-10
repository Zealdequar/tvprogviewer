using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the nopCommerce news
    /// </summary>
    public class TvProgNewsViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IHomeModelFactory _homeModelFactory;

        #endregion

        #region Ctor

        public TvProgNewsViewComponent(IHomeModelFactory homeModelFactory)
        {
            _homeModelFactory = homeModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke()
        {
            try
            {
                //prepare model
                var model = _homeModelFactory.PrepareTvProgNewsModel();

                return View(model);
            }
            catch
            {
                return Content(string.Empty);
            }
        }

        #endregion
    }
}