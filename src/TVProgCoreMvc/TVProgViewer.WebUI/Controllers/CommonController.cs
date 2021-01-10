using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.TvProgMain;
using TVProgViewer.Web.Framework.Mvc.Filters;

namespace TVProgViewer.WebUI.Controllers
{
    public class CommonController : BasePublicController
    {
        #region Поля
        
        private readonly CommonSettings _commonSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IProgrammeService _programmeService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        #endregion

        #region Конструктор

        public CommonController(CommonSettings commonSettings,
             ICurrencyService currencyService,
             IProgrammeService programmeService,
             ILogger logger,
             IWorkContext workContext)
        {
            _commonSettings = commonSettings;
            _currencyService = currencyService;
            _programmeService = programmeService;
            _logger = logger;
            _workContext = workContext;
        }

        #endregion

        #region Методы

        // Страницы не существует
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";
            
            return View();
        }

        // Установка ТВ-провайдера       
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetProvider(int customerProvider, string returnUrl = "")
        {
            var provider = _programmeService.GetProviderById(customerProvider);
            if (provider != null)
                _workContext.WorkingProvider = provider;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        // Установка типа ТВ-программы
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetTypeProg(int customerTypeProg, string returnUrl = "")
        {
            var typeProg = _programmeService.GetTypeProgById(customerTypeProg);
            if (typeProg != null)
                _workContext.WorkingTypeProg = typeProg;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        // Установка категории ТВ-программы
        [CheckAccessPublicStore(true)]
        public virtual IActionResult SetCategory(string customerCategory, string returnUrl = "")
        {
            var categories = _programmeService.GetCategories();
            categories.Insert(0, "Все категории");
            var category = categories.SingleOrDefault(x => x == customerCategory);
            if (category != null)
                _workContext.WorkingCategory = category;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }
        #endregion
    }
}