using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class ExternalMethodsViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;

        #endregion

        #region Ctor

        public ExternalMethodsViewComponent(IExternalAuthenticationModelFactory externalAuthenticationModelFactory)
        {
            _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke()
        {
            var model = _externalAuthenticationModelFactory.PrepareExternalMethodsModel();

            return View(model);
        }

        #endregion
    }
}
