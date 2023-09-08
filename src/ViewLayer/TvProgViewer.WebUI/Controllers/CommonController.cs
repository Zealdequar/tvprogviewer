using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Http;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Themes;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Sitemap;
using TvProgViewer.Services.TvProgMain;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CommonController : BasePublicController
    {
        #region Поля

        private readonly CaptchaSettings _captchaSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly IUserActivityService _userActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISitemapModelFactory _sitemapModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly SitemapSettings _sitemapSettings;
        private readonly SitemapXmlSettings _sitemapXmlSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IProgrammeService _programmeService;

        #endregion

        #region Конструктор

        public CommonController(CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
            IUserActivityService userActivityService,
            IGenericAttributeService genericAttributeService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISitemapModelFactory sitemapModelFactory,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IVendorService vendorService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings,
            IProgrammeService programmeService)
        {
            _captchaSettings = captchaSettings;
            _commonSettings = commonSettings;
            _commonModelFactory = commonModelFactory;
            _currencyService = currencyService;
            _userActivityService = userActivityService;
            _genericAttributeService = genericAttributeService;
            _htmlFormatter = htmlFormatter;
            _languageService = languageService;
            _localizationService = localizationService;
            _sitemapModelFactory = sitemapModelFactory;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _vendorService = vendorService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _sitemapSettings = sitemapSettings;
            _sitemapXmlSettings = sitemapXmlSettings;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
            _programmeService = programmeService;
        }

        #endregion

        #region Методы

        // Страницы не существует:
        public virtual IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }

        // Установка ТВ-провайдера       
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetProvider(int userProvider, string returnUrl = "")
        {
            var provider = await _programmeService.GetProviderByIdAsync(userProvider);
            if (provider != null)
                await _workContext.SetWorkingProviderAsync(provider);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        // Установка типа ТВ-программы
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetTypeProg(int userTypeProg, string returnUrl = "")
        {
            var typeProg = await _programmeService.GetTypeProgByIdAsync(userTypeProg);
            if (typeProg != null)
                await _workContext.SetWorkingTypeProgAsync(typeProg);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        // Установка категории ТВ-программы
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> SetCategory(string userCategory, string returnUrl = "")
        {
            var categories = await _programmeService.GetCategoriesAsync();
            categories.Insert(0, "Все категории");
            var category = categories.SingleOrDefault(x => x == userCategory);
            if (category != null)
                await _workContext.SetWorkingCategoryAsync(category);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
        {
            var language = await _languageService.GetLanguageByIdAsync(langid);
            if (!language?.Published ?? false)
                language = await _workContext.GetWorkingLanguageAsync();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //language part in URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //remove current language code if it's already localized URL
                if ((await returnUrl.IsLocalizedUrlAsync(Request.PathBase, true)).IsLocalized)
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

                //and add code of passed language
                returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, language);
            }

            await _workContext.SetWorkingLanguageAsync(language);

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> SetCurrency(int userCurrency, string returnUrl = "")
        {
            var currency = await _currencyService.GetCurrencyByIdAsync(userCurrency);
            if (currency != null)
                await _workContext.SetWorkingCurrencyAsync(currency);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> SetTaxType(int userTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), userTaxType);
            await _workContext.SetTaxDisplayTypeAsync(taxDisplayType);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        //contact us page
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> ContactUs()
        {
            var model = new ContactUsModel();
            model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);

            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

            if (ModelState.IsValid)
            {
                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

                await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                await _userActivityService.InsertActivityAsync("PublicStore.ContactUs",
                    await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }

            return View(model);
        }

        //contact vendor page
        public virtual async Task<IActionResult> ContactVendor(int vendorId)
        {
            if (!_vendorSettings.AllowUsersToContactVendors)
                return RedirectToRoute("Homepage");

            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("Homepage");

            var model = new ContactVendorModel();
            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, false);

            return View(model);
        }

        [HttpPost, ActionName("ContactVendor")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ContactVendorSend(ContactVendorModel model, bool captchaValid)
        {
            if (!_vendorSettings.AllowUsersToContactVendors)
                return RedirectToRoute("Homepage");

            var vendor = await _vendorService.GetVendorByIdAsync(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, vendor, true);

            if (ModelState.IsValid)
            {
                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = _htmlFormatter.FormatText(model.Enquiry, false, true, false, false, false, false);

                await _workflowMessageService.SendContactVendorMessageAsync(vendor, (await _workContext.GetWorkingLanguageAsync()).Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = await _localizationService.GetResourceAsync("ContactVendor.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        //sitemap page
        public virtual async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
        {
            if (!_sitemapSettings.SitemapEnabled)
                return RedirectToRoute("Homepage");

            var model = await _sitemapModelFactory.PrepareSitemapModelAsync(pageModel);

            return View(model);
        }

        //SEO sitemap page
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SitemapXml(int? id)
        {
            if (!_sitemapXmlSettings.SitemapXmlEnabled)
                return StatusCode(StatusCodes.Status403Forbidden);

            try
            {
                var sitemapXmlModel = await _sitemapModelFactory.PrepareSitemapXmlModelAsync(id ?? 0);
                return PhysicalFile(sitemapXmlModel.SitemapXmlPath, MimeTypes.ApplicationXml);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        public virtual async Task<IActionResult> SetStoreTheme(string themeName, string returnUrl = "")
        {
            await _themeContext.SetWorkingThemeNameAsync(themeName);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual async Task<IActionResult> EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            var store = await _storeContext.GetCurrentStoreAsync();
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.EuCookieLawAcceptedAttribute, true, store.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> RobotsTextFile()
        {
            var robotsFileContent = await _commonModelFactory.PrepareRobotsTextFileAsync();

            return Content(robotsFileContent, MimeTypes.TextPlain);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(ignore: true)]
        public virtual IActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [CheckAccessClosedStore(ignore: true)]
        public virtual IActionResult StoreClosed()
        {
            return View();
        }

        //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
        public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
        {
            //ensure it's invoked from our GenericPathRoute class
            if (!HttpContext.Items.TryGetValue(TvProgHttpDefaults.GenericRouteInternalRedirect, out var value) || value is not bool redirect || !redirect)
            {
                url = Url.RouteUrl("Homepage");
                permanentRedirect = false;
            }

            //home page
            if (string.IsNullOrEmpty(url))
            {
                url = Url.RouteUrl("Homepage");
                permanentRedirect = false;
            }

            //prevent open redirection attack
            if (!Url.IsLocalUrl(url))
            {
                url = Url.RouteUrl("Homepage");
                permanentRedirect = false;
            }

            if (permanentRedirect)
                return RedirectPermanent(url);

            return Redirect(url);
        }

        #endregion
    }
}