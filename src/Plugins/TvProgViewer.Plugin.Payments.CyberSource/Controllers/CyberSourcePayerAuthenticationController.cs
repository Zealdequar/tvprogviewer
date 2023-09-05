using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Plugin.Payments.CyberSource.Services;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.WebUI.Controllers;

namespace TvProgViewer.Plugin.Payments.CyberSource.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CyberSourcePayerAuthenticationController : BasePublicController
    {
        #region Fields

        private readonly UserTokenService _userTokenService;
        private readonly CyberSourceService _cyberSourceService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CyberSourcePayerAuthenticationController(UserTokenService userTokenService,
            CyberSourceService cyberSourceService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _userTokenService = userTokenService;
            _cyberSourceService = cyberSourceService;
            _localizationService = localizationService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Setup(int userTokenId, string transientToken)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var user = await _workContext.GetCurrentUserAsync();
            var userToken = await _userTokenService.GetByIdAsync(userTokenId);
            if (string.IsNullOrEmpty(transientToken) && (userToken is null || userToken.UserId != user.Id))
                return Json(new { success = false, message = error });

            var (result, _) = await _cyberSourceService.PayerAuthenticationSetupAsync(userToken, transientToken);
            if (result?.ConsumerAuthenticationInformation is null || result.Status == CyberSourceDefaults.PayerAuthenticationSetupStatus.Failed)
            {
                var message = $"Payer authentication setup failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", user: user);

                return Json(new { success = false, message = error });
            }

            return Json(new
            {
                success = true,
                accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                deviceDataCollectionUrl = result.ConsumerAuthenticationInformation.DeviceDataCollectionUrl,
                referenceId = result.ConsumerAuthenticationInformation.ReferenceId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Enrollment(string referenceId, int userTokenId, string transientToken)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var user = await _workContext.GetCurrentUserAsync();
            var userToken = await _userTokenService.GetByIdAsync(userTokenId);
            if (string.IsNullOrEmpty(transientToken) && (userToken is null || userToken.UserId != user.Id))
                return Json(new { success = false, message = error });

            var store = await _storeContext.GetCurrentStoreAsync();
            var returnUrl = $"{store.Url.TrimEnd('/')}{Url.RouteUrl(CyberSourceDefaults.PayerRedirectRouteName)}".ToLowerInvariant();
            var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);
            var (shoppingCartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
            var request = new ProcessPaymentRequest
            {
                OrderGuid = Guid.NewGuid(),
                UserId = user.Id,
                OrderTotal = shoppingCartTotal ?? decimal.Zero
            };
            var (result, _) = await _cyberSourceService.PayerAuthenticationEnrollmentAsync(referenceId, userToken, transientToken, request, returnUrl);
            if (result?.ConsumerAuthenticationInformation is null || result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Failed)
            {
                var message = $"Payer authentication enrollment failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", user: user);

                if (result?.ConsumerAuthenticationInformation is not null && 
                    result.ErrorInformation?.Reason == CyberSourceDefaults.PayerAuthenticationErrorReason.ConsumerAuthenticationRequired)
                {
                    return Json(new
                    {
                        success = true,
                        complete = false,
                        authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId,
                        accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                        stepUpUrl = result.ConsumerAuthenticationInformation.StepUpUrl
                    });
                }

                return Json(new { success = false, message = result?.ConsumerAuthenticationInformation?.CardholderMessage ?? error });
            }

            if (result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Pending ||
                result.ConsumerAuthenticationInformation.ChallengeRequired == "Y")
            {
                return Json(new
                {
                    success = true,
                    complete = false,
                    authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId,
                    accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                    stepUpUrl = result.ConsumerAuthenticationInformation.StepUpUrl
                });
            }

            if (result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Success)
            {
                return Json(new
                {
                    success = true,
                    complete = true,
                    authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId
                });
            }

            return Json(new { success = false, message = error });
        }

        [HttpPost]
        public async Task<IActionResult> Validate(string authenticationTransactionId)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var user = await _workContext.GetCurrentUserAsync();
            if (string.IsNullOrEmpty(authenticationTransactionId))
                return Json(new { success = false, message = error });

            var (result, _) = await _cyberSourceService.PayerAuthenticationValidateAsync(authenticationTransactionId);
            if (result?.Status != CyberSourceDefaults.PayerAuthenticationStatus.Success)
            {
                var message = $"Payer authentication validation failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", user: user);

                return Json(new { success = false, message = error });
            }

            return Json(new { success = true });
        }

        #endregion
    }
}