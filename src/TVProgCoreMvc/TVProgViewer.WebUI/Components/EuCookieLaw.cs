using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Http;
using TVProgViewer.Services.Common;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Components
{
    public class EuCookieLawViewComponent : TvProgViewComponent
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly StoreInformationSettings _storeInformationSettings;

        public EuCookieLawViewComponent(IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IWorkContext workContext,
            StoreInformationSettings storeInformationSettings)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _workContext = workContext;
            _storeInformationSettings = storeInformationSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if ((await _workContext.GetCurrentUserAsync()).IsSearchEngineAccount())
                return Content("");

            if (await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.EuCookieLawAcceptedAttribute, (await _storeContext.GetCurrentStoreAsync()).Id))
                //already accepted
                return Content("");

            //ignore notification?
            //right now it's used during logout so popup window is not displayed twice
            if (TempData[$"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.IgnoreEuCookieLawWarning}"] != null && Convert.ToBoolean(TempData[$"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.IgnoreEuCookieLawWarning}"]))
                return Content("");

            return View();
        }
    }
}