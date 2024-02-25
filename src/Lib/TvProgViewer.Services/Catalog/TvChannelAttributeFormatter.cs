using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Tax;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute formatter
    /// </summary>
    public partial class TvChannelAttributeFormatter : ITvChannelAttributeFormatter
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDownloadService _downloadService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public TvChannelAttributeFormatter(ICurrencyService currencyService,
            IDownloadService downloadService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IStoreContext storeContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _currencyService = currencyService;
            _downloadService = downloadService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(TvChannel tvchannel, string attributesXml)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            
            return await FormatAttributesAsync(tvchannel, attributesXml, user, currentStore);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderTvChannelAttributes">A value indicating whether to render tvchannel attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(TvChannel tvchannel, string attributesXml,
            User user, Store store, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderTvChannelAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            //attributes
            if (renderTvChannelAttributes)
            {
                foreach (var attribute in await _tvchannelAttributeParser.ParseTvChannelAttributeMappingsAsync(attributesXml))
                {
                    var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId);
                    var attributeName = await _localizationService.GetLocalizedAsync(tvchannelAttribute, a => a.Name, currentLanguage.Id);

                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _tvchannelAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(value, false, true, false, false, false, false)}";
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                _ = Guid.TryParse(value, out var downloadGuid);
                                var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = WebUtility.HtmlEncode(fileName);

                                    var attributeText = allowHyperlinks ? $"<a href=\"{_webHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}\" class=\"fileuploadattribute\">{fileName}</a>"
                                        : fileName;

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = WebUtility.HtmlEncode(attributeName);

                                    formattedAttribute = $"{attributeName}: {attributeText}";
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = $"{attributeName}: {value}";

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                            }

                            if (string.IsNullOrEmpty(formattedAttribute))
                                continue;

                            if (result.Length > 0)
                                result.Append(separator);
                            result.Append(formattedAttribute);
                        }
                    }
                    //tvchannel attribute values
                    else
                    {
                        foreach (var attributeValue in await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = $"{attributeName}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";

                            if (renderPrices)
                            {
                                if (attributeValue.PriceAdjustmentUsePercentage)
                                {
                                    if (attributeValue.PriceAdjustment > decimal.Zero)
                                    {
                                        formattedAttribute += string.Format(
                                                await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                                "+", attributeValue.PriceAdjustment.ToString("G29"), "%");
                                    }
                                    else if (attributeValue.PriceAdjustment < decimal.Zero)
                                    {
                                        formattedAttribute += string.Format(
                                                await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                                string.Empty, attributeValue.PriceAdjustment.ToString("G29"), "%");
                                    }
                                }
                                else
                                {
                                    var attributeValuePriceAdjustment = await _priceCalculationService.GetTvChannelAttributeValuePriceAdjustmentAsync(tvchannel, attributeValue, user, store);
                                    var (priceAdjustmentBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, attributeValuePriceAdjustment, user);
                                    var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());

                                    if (priceAdjustmentBase > decimal.Zero)
                                    {
                                        formattedAttribute += string.Format(
                                                await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                                "+", await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false), string.Empty);
                                    }
                                    else if (priceAdjustmentBase < decimal.Zero)
                                    {
                                        formattedAttribute += string.Format(
                                                await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                                "-", await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false), string.Empty);
                                    }
                                }
                            }

                            //display quantity
                            if (_shoppingCartSettings.RenderAssociatedAttributeValueQuantity && attributeValue.AttributeValueType == AttributeValueType.AssociatedToTvChannel)
                            {
                                //render only when more than 1
                                if (attributeValue.Quantity > 1)
                                    formattedAttribute += string.Format(await _localizationService.GetResourceAsync("TvChannelAttributes.Quantity"), attributeValue.Quantity);
                            }

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

                            if (string.IsNullOrEmpty(formattedAttribute))
                                continue;

                            if (result.Length > 0)
                                result.Append(separator);
                            result.Append(formattedAttribute);
                        }
                    }
                }
            }

            //gift cards
            if (!renderGiftCardAttributes)
                return result.ToString();

            if (!tvchannel.IsGiftCard)
                return result.ToString();

            _tvchannelAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

            //sender
            var giftCardFrom = tvchannel.GiftCardType == GiftCardType.Virtual ?
                string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.From.Virtual"), giftCardSenderName, giftCardSenderEmail) :
                string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.From.Physical"), giftCardSenderName);
            //recipient
            var giftCardFor = tvchannel.GiftCardType == GiftCardType.Virtual ?
                string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.For.Virtual"), giftCardRecipientName, giftCardRecipientEmail) :
                string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.For.Physical"), giftCardRecipientName);

            //encode (if required)
            if (htmlEncode)
            {
                giftCardFrom = WebUtility.HtmlEncode(giftCardFrom);
                giftCardFor = WebUtility.HtmlEncode(giftCardFor);
            }

            if (!string.IsNullOrEmpty(result.ToString()))
            {
                result.Append(separator);
            }

            result.Append(giftCardFrom);
            result.Append(separator);
            result.Append(giftCardFor);

            return result.ToString();
        }

        #endregion
    }
}