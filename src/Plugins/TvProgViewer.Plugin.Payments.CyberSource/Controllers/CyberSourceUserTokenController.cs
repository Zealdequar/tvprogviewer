using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Plugin.Payments.CyberSource.Domain;
using TvProgViewer.Plugin.Payments.CyberSource.Models;
using TvProgViewer.Plugin.Payments.CyberSource.Services;
using TvProgViewer.Plugin.Payments.CyberSource.Services.Helpers;
using TvProgViewer.Services.Users;
using TvProgViewer.WebUI.Controllers;

namespace TvProgViewer.Plugin.Payments.CyberSource.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CyberSourceUserTokenController : BasePublicController
    {
        #region Fields

        private readonly UserTokenService _userTokenService;
        private readonly CyberSourceService _cyberSourceService;
        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CyberSourceUserTokenController(UserTokenService userTokenService,
            CyberSourceService cyberSourceService,
            CyberSourceSettings cyberSourceSettings,
            IUserService userService,
            IWorkContext workContext)
        {
            _userTokenService = userTokenService;
            _cyberSourceService = cyberSourceService;
            _cyberSourceSettings = cyberSourceSettings;
            _userService = userService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> UserTokens()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("UserInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("UserInfo");

            var model = new UserTokenListModel();
            var tokens = await _userTokenService.GetAllTokensAsync(userId: user.Id);
            foreach (var token in tokens)
            {
                var tokenModel = new UserTokenListModel.UserTokenDetailsModel
                {
                    ThreeDigitCardType = token.ThreeDigitCardType,
                    CardExpirationMonth = token.CardExpirationMonth,
                    CardExpirationYear = token.CardExpirationYear,
                    CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX"),
                    Id = token.Id
                };

                model.Tokens.Add(tokenModel);
            }

            return View("~/Plugins/Payments.CyberSource/Views/UserToken/UserTokens.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenDelete(int tokenId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //find token (ensure that it belongs to the current user)
            var token = await _userTokenService.GetByIdAsync(tokenId);
            if (token != null && token.UserId == user.Id)
            {
                var (_, error) = await _cyberSourceService.DeletePaymentInstrumentAsync(token.SubscriptionId);
                if (!string.IsNullOrEmpty(error))
                    throw new TvProgException(error);

                await _userTokenService.DeleteAsync(token);
            }

            //redirect to the token list page
            return Json(new
            {
                redirect = Url.RouteUrl(CyberSourceDefaults.UserTokensRouteName)
            });
        }

        public async Task<IActionResult> TokenAdd()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("UserInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("UserInfo");

            var model = new UserTokenEditModel();

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.UserToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.UserToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.CyberSource/Views/UserToken/TokenAdd.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenAdd(UserTokenEditModel model)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (ModelState.IsValid)
            {
                var cardNumber = CreditCardHelper.RemoveSpecialCharacters(model.UserToken.CardNumber);
                var firstSixDigitsOfCard = CreditCardHelper.GetFirstSixDigitsOfCard(cardNumber);
                var lastFourDigitsOfCard = CreditCardHelper.GetLastFourDigitsOfCard(cardNumber);
                var existingTokens = await _userTokenService.GetAllTokensAsync(user.Id);
                if (existingTokens.Any(token => token.FirstSixDigitOfCard == firstSixDigitsOfCard && token.LastFourDigitOfCard == lastFourDigitsOfCard))
                    throw new TvProgException("Token already exists!");

                var (instrumentResult, instrumentError) = await _cyberSourceService.CreateInstrumentIdAsync(cardNumber);
                if (!string.IsNullOrEmpty(instrumentError))
                    throw new TvProgException(instrumentError);

                var instrumentToken = instrumentResult?.Id;
                if (!string.IsNullOrEmpty(instrumentToken))
                {
                    var (paymentInstrumentResult, paymentInstrumentError) = await _cyberSourceService.CreatePaymentInstrumentAsync(
                        instrumentIdentifier: instrumentToken,
                        cardExpirationMonth: model.UserToken.ExpireMonth.PadLeft(2, '0'),
                        cardExpirationYear: model.UserToken.ExpireYear,
                        cardType: CreditCardHelper.GetCardTypeByNumber(cardNumber));

                    if (!string.IsNullOrEmpty(paymentInstrumentError))
                        throw new TvProgException(paymentInstrumentError);

                    if (paymentInstrumentResult != null)
                    {
                        await _userTokenService.InsertAsync(new CyberSourceUserToken
                        {
                            UserId = user.Id,
                            ThreeDigitCardType = CreditCardHelper.GetThreeDigitCardTypeByNumber(model.UserToken.CardNumber),
                            CardExpirationMonth = paymentInstrumentResult.Card?.ExpirationMonth,
                            CardExpirationYear = paymentInstrumentResult.Card?.ExpirationYear,
                            FirstSixDigitOfCard = firstSixDigitsOfCard,
                            LastFourDigitOfCard = lastFourDigitsOfCard,
                            InstrumentIdentifier = instrumentToken,
                            InstrumentIdentifierStatus = instrumentResult.State,
                            SubscriptionId = paymentInstrumentResult.Id
                        });
                    }
                }

                return RedirectToRoute(CyberSourceDefaults.UserTokensRouteName);
            }

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.UserToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.UserToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.CyberSource/Views/UserToken/TokenAdd.cshtml", model);
        }

        public async Task<IActionResult> TokenEdit(int tokenId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("UserInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("UserInfo");

            //find token (ensure that it belongs to the current user)
            var token = await _userTokenService.GetByIdAsync(tokenId);
            if (token == null || token.UserId != user.Id)
                return RedirectToRoute(CyberSourceDefaults.UserTokensRouteName);

            var model = new UserTokenEditModel();

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.UserToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.UserToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            model.UserToken.Id = token.Id;
            model.UserToken.CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX");
            model.UserToken.ExpireMonth = token.CardExpirationMonth;
            var selectedMonth = model.UserToken.ExpireMonths.FirstOrDefault(x => x.Value.Equals(model.UserToken.ExpireMonth, StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            model.UserToken.ExpireYear = token.CardExpirationYear;
            var selectedYear = model.UserToken.ExpireYears.FirstOrDefault(x => x.Value.Equals(model.UserToken.ExpireYear, StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            return View("~/Plugins/Payments.CyberSource/Views/UserToken/TokenEdit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenEdit(UserTokenEditModel model, int tokenId)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //find token (ensure that it belongs to the current user)
            var token = await _userTokenService.GetByIdAsync(tokenId);
            if (token == null || token.UserId != user.Id)
                return RedirectToRoute(CyberSourceDefaults.UserTokensRouteName);

            if (ModelState.IsValid)
            {
                var (result, error) = await _cyberSourceService.UpdatePaymentInstrumentAsync(paymentInstrumentTokenId: token.SubscriptionId,
                    instrumentIdentifier: token.InstrumentIdentifier,
                    cardExpirationMonth: model.UserToken.ExpireMonth.PadLeft(2, '0'),
                    cardExpirationYear: model.UserToken.ExpireYear);
                if (!string.IsNullOrEmpty(error))
                    throw new TvProgException(error);

                if (result != null)
                {
                    token.CardExpirationMonth = result.Card.ExpirationMonth;
                    token.CardExpirationYear = model.UserToken.ExpireYear;
                    await _userTokenService.UpdateAsync(token);

                    return RedirectToRoute(CyberSourceDefaults.UserTokensRouteName);
                }
            }

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.UserToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.UserToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            model.UserToken.CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX");

            return View("~/Plugins/Payments.CyberSource/Views/UserToken/TokenEdit.cshtml", model);
        }

        #endregion
    }
}