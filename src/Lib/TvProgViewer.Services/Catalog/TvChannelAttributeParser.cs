using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute parser
    /// </summary>
    public partial class TvChannelAttributeParser : BaseAttributeParser, ITvChannelAttributeParser
    {

        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly IRepository<TvChannelAttributeValue> _tvchannelAttributeValueRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TvChannelAttributeParser(ICurrencyService currencyService,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            ITvChannelAttributeService tvchannelAttributeService,
            IRepository<TvChannelAttributeValue> tvchannelAttributeValueRepository,
            IWorkContext workContext)
        {
            _currencyService = currencyService;
            _downloadService = downloadService;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelAttributeValueRepository = tvchannelAttributeValueRepository;
            _workContext = workContext;
            _localizationService = localizationService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns a list which contains all possible combinations of elements
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="elements">Elements to make combinations</param>
        /// <returns>All possible combinations of elements</returns>
        protected virtual IList<IList<T>> CreateCombination<T>(IList<T> elements)
        {
            var rez = new List<IList<T>>();

            for (var i = 1; i < Math.Pow(2, elements.Count); i++)
            {
                var current = new List<T>();
                var index = -1;

                //transform int to binary string
                var binaryMask = Convert.ToString(i, 2).PadLeft(elements.Count, '0');

                foreach (var flag in binaryMask)
                {
                    index++;

                    if (flag == '0')
                        continue;

                    //add element if binary mask in the position of element has 1
                    current.Add(elements[index]);
                }

                rez.Add(current);
            }

            return rez;
        }
        
        /// <summary>
        /// Gets selected tvchannel attribute values with the quantity entered by the user
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvchannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>Collections of pairs of tvchannel attribute values and their quantity</returns>
        protected IList<Tuple<string, string>> ParseValuesWithQuantity(string attributesXml, int tvchannelAttributeMappingId)
        {
            var selectedValues = new List<Tuple<string, string>>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                foreach (XmlNode attributeNode in xmlDoc.SelectNodes(@"//Attributes/TvChannelAttribute"))
                {
                    if (attributeNode.Attributes?["ID"] == null)
                        continue;

                    if (!int.TryParse(attributeNode.Attributes["ID"].InnerText.Trim(), out var attributeId) ||
                        attributeId != tvchannelAttributeMappingId)
                        continue;

                    foreach (XmlNode attributeValue in attributeNode.SelectNodes("TvChannelAttributeValue"))
                    {
                        var value = attributeValue.SelectSingleNode("Value").InnerText.Trim();
                        var quantityNode = attributeValue.SelectSingleNode("Quantity");
                        selectedValues.Add(new Tuple<string, string>(value, quantityNode != null ? quantityNode.InnerText.Trim() : string.Empty));
                    }
                }
            }
            catch
            {
                // ignored
            }

            return selectedValues;
        }

        /// <summary>
        /// Adds gift cards attributes in XML format
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        protected virtual void AddGiftCardsAttributesXml(TvChannel tvchannel, IFormCollection form, ref string attributesXml)
        {
            if (!tvchannel.IsGiftCard)
                return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{tvchannel.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{tvchannel.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{tvchannel.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{tvchannel.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{tvchannel.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        /// <summary>
        /// Gets tvchannel attributes in XML format
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <param name="errors">Errors</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes in XML format
        /// </returns>
        protected virtual async Task<string> GetTvChannelAttributesXmlAsync(TvChannel tvchannel, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            var tvchannelAttributes = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
            foreach (var attribute in tvchannelAttributes)
            {
                var controlId = $"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by user
                                    var quantity = 1;
                                    var quantityStr = form[$"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                        (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(await _localizationService.GetResourceAsync("TvChannels.QuantityShouldBePositive"));

                                    attributesXml = AddTvChannelAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by user
                                        var quantity = 1;
                                        var quantityStr = form[$"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(await _localizationService.GetResourceAsync("TvChannels.QuantityShouldBePositive"));

                                        attributesXml = AddTvChannelAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by user
                                var quantity = 1;
                                var quantityStr = form[$"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(await _localizationService.GetResourceAsync("TvChannels.QuantityShouldBePositive"));

                                attributesXml = AddTvChannelAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = AddTvChannelAttribute(attributesXml, attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                            }
                            catch
                            {
                                // ignored
                            }

                            if (selectedDate.HasValue)
                                attributesXml = AddTvChannelAttribute(attributesXml, attribute, selectedDate.Value.ToString("D"));
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            _ = Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                                attributesXml = AddTvChannelAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in tvchannelAttributes)
            {
                var conditionMet = await IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = RemoveTvChannelAttribute(attributesXml, attribute);
                }
            }
            return attributesXml;
        }

        #endregion

        #region TvChannel attributes

        /// <summary>
        /// Gets selected tvchannel attribute mappings
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the selected tvchannel attribute mappings
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeMapping>> ParseTvChannelAttributeMappingsAsync(string attributesXml)
        {
            var result = new List<TvChannelAttributeMapping>();
            if (string.IsNullOrEmpty(attributesXml))
                return result;

            var ids = ParseAttributeIds(attributesXml);
            foreach (var id in ids)
            {
                var attribute = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(id);
                if (attribute != null) 
                    result.Add(attribute);
            }

            return result;
        }

        /// <summary>
        /// /// Get tvchannel attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvchannelAttributeMappingId">TvChannel attribute mapping identifier; pass 0 to load all values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute values
        /// </returns>
        public virtual async Task<IList<TvChannelAttributeValue>> ParseTvChannelAttributeValuesAsync(string attributesXml, int tvchannelAttributeMappingId = 0)
        {
            var values = new List<TvChannelAttributeValue>();
            if (string.IsNullOrEmpty(attributesXml))
                return values;

            var attributes = await ParseTvChannelAttributeMappingsAsync(attributesXml);

            //to load values only for the passed tvchannel attribute mapping
            if (tvchannelAttributeMappingId > 0)
                attributes = attributes.Where(attribute => attribute.Id == tvchannelAttributeMappingId).ToList();

            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                foreach (var attributeValue in ParseValuesWithQuantity(attributesXml, attribute.Id))
                {
                    if (string.IsNullOrEmpty(attributeValue.Item1) || !int.TryParse(attributeValue.Item1, out var attributeValueId))
                        continue;

                    var value = await _tvchannelAttributeService.GetTvChannelAttributeValueByIdAsync(attributeValueId);
                    if (value == null)
                        continue;

                    if (!string.IsNullOrEmpty(attributeValue.Item2) && int.TryParse(attributeValue.Item2, out var quantity) && quantity != value.Quantity)
                    {
                        //if user enters quantity, use new entity with new quantity

                        var oldValue = await _tvchannelAttributeValueRepository.LoadOriginalCopyAsync(value);

                        oldValue.TvChannelAttributeMappingId = attribute.Id;
                        oldValue.Quantity = quantity;
                        values.Add(oldValue);
                    }
                    else
                        values.Add(value);
                }
            }

            return values;
        }

        /// <summary>
        /// Gets selected tvchannel attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvchannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>TvChannel attribute values</returns>
        public virtual IList<string> ParseValues(string attributesXml, int tvchannelAttributeMappingId)
        {
            var selectedValues = new List<string>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/TvChannelAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != tvchannelAttributeMappingId)
                        continue;

                    var nodeList2 = node1.SelectNodes(@"TvChannelAttributeValue/Value");
                    foreach (XmlNode node2 in nodeList2)
                    {
                        var value = node2.InnerText.Trim();
                        selectedValues.Add(value);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return selectedValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="value">Value</param>
        /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToTvChannel to specify the quantity entered by the user)</param>
        /// <returns>Updated result (XML format)</returns>
        public virtual string AddTvChannelAttribute(string attributesXml, TvChannelAttributeMapping tvchannelAttributeMapping, string value, int? quantity = null)
        {
            var result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributesXml);
                }

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement attributeElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/TvChannelAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != tvchannelAttributeMapping.Id)
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("TvChannelAttribute");
                    attributeElement.SetAttribute("ID", tvchannelAttributeMapping.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("TvChannelAttributeValue");
                attributeElement.AppendChild(attributeValueElement);

                var attributeValueValueElement = xmlDoc.CreateElement("Value");
                attributeValueValueElement.InnerText = value;
                attributeValueElement.AppendChild(attributeValueValueElement);

                //the quantity entered by the user
                if (quantity.HasValue)
                {
                    var attributeValueQuantity = xmlDoc.CreateElement("Quantity");
                    attributeValueQuantity.InnerText = quantity.ToString();
                    attributeValueElement.AppendChild(attributeValueQuantity);
                }

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Updated result (XML format)</returns>
        public virtual string RemoveTvChannelAttribute(string attributesXml, TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            return RemoveAttribute(attributesXml, tvchannelAttributeMapping.Id);
        }

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributesXml1">The attributes of the first tvchannel</param>
        /// <param name="attributesXml2">The attributes of the second tvchannel</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreQuantity">A value indicating whether we should ignore the quantity of attribute value entered by the user</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> AreTvChannelAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true)
        {
            var attributes1 = await ParseTvChannelAttributeMappingsAsync(attributesXml1);
            if (ignoreNonCombinableAttributes) 
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();

            var attributes2 = await ParseTvChannelAttributeMappingsAsync(attributesXml2);
            if (ignoreNonCombinableAttributes) 
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();

            if (attributes1.Count != attributes2.Count)
                return false;

            var attributesEqual = true;
            foreach (var a1 in attributes1)
            {
                var hasAttribute = false;
                foreach (var a2 in attributes2)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    hasAttribute = true;
                    var values1Str = ParseValuesWithQuantity(attributesXml1, a1.Id);
                    var values2Str = ParseValuesWithQuantity(attributesXml2, a2.Id);
                    if (values1Str.Count == values2Str.Count)
                    {
                        foreach (var str1 in values1Str)
                        {
                            var hasValue = false;
                            foreach (var str2 in values2Str)
                            {
                                //case insensitive? 
                                //if (str1.Trim().ToLowerInvariant() == str2.Trim().ToLowerInvariant())
                                if (str1.Item1.Trim() != str2.Item1.Trim())
                                    continue;

                                hasValue = ignoreQuantity || str1.Item2.Trim() == str2.Item2.Trim();
                                break;
                            }

                            if (hasValue)
                                continue;

                            attributesEqual = false;
                            break;
                        }
                    }
                    else
                    {
                        attributesEqual = false;
                        break;
                    }
                }

                if (hasAttribute)
                    continue;

                attributesEqual = false;
                break;
            }

            return attributesEqual;
        }

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="pam">TvChannel attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool?> IsConditionMetAsync(TvChannelAttributeMapping pam, string selectedAttributesXml)
        {
            if (pam == null)
                throw new ArgumentNullException(nameof(pam));

            var conditionAttributeXml = pam.ConditionAttributeXml;
            if (string.IsNullOrEmpty(conditionAttributeXml))
                //no condition
                return null;

            //load an attribute this one depends on
            var dependOnAttribute = (await ParseTvChannelAttributeMappingsAsync(conditionAttributeXml)).FirstOrDefault();
            if (dependOnAttribute == null)
                return true;

            var valuesThatShouldBeSelected = ParseValues(conditionAttributeXml, dependOnAttribute.Id)
                //a workaround here:
                //ConditionAttributeXml can contain "empty" values (nothing is selected)
                //but in other cases (like below) we do not store empty values
                //that's why we remove empty values here
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
            var selectedValues = ParseValues(selectedAttributesXml, dependOnAttribute.Id);
            if (valuesThatShouldBeSelected.Count != selectedValues.Count)
                return false;

            //compare values
            var allFound = true;
            foreach (var t1 in valuesThatShouldBeSelected)
            {
                var found = false;
                foreach (var t2 in selectedValues)
                    if (t1 == t2)
                        found = true;
                if (!found)
                    allFound = false;
            }

            return allFound;
        }

        /// <summary>
        /// Finds a tvchannel attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the found tvchannel attribute combination
        /// </returns>
        public virtual async Task<TvChannelAttributeCombination> FindTvChannelAttributeCombinationAsync(TvChannel tvchannel,
            string attributesXml, bool ignoreNonCombinableAttributes = true)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //anyway combination cannot contains non combinable attributes
            if (string.IsNullOrEmpty(attributesXml))
                return null;

            var combinations = await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id);
            return await combinations.FirstOrDefaultAwaitAsync(async x =>
                await AreTvChannelAttributesEqualAsync(x.AttributesXml, attributesXml, ignoreNonCombinableAttributes));
        }

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="allowedAttributeIds">List of allowed attribute identifiers. If null or empty then all attributes would be used.</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attribute combinations in XML format
        /// </returns>
        public virtual async Task<IList<string>> GenerateAllCombinationsAsync(TvChannel tvchannel, bool ignoreNonCombinableAttributes = false, IList<int> allowedAttributeIds = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var allTvChannelAttributeMappings = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
            
            if (ignoreNonCombinableAttributes) 
                allTvChannelAttributeMappings = allTvChannelAttributeMappings.Where(x => !x.IsNonCombinable()).ToList();

            //get all possible attribute combinations
            var allPossibleAttributeCombinations = CreateCombination(allTvChannelAttributeMappings);

            var allAttributesXml = new List<string>();

            foreach (var combination in allPossibleAttributeCombinations)
            {
                var attributesXml = new List<string>();
                foreach (var tvchannelAttributeMapping in combination)
                {
                    if (!tvchannelAttributeMapping.ShouldHaveValues())
                        continue;

                    //get tvchannel attribute values
                    var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(tvchannelAttributeMapping.Id);

                    //filter tvchannel attribute values
                    if (allowedAttributeIds?.Any() ?? false) 
                        attributeValues = attributeValues.Where(attributeValue => allowedAttributeIds.Contains(attributeValue.Id)).ToList();

                    if (!attributeValues.Any())
                        continue;

                    var isCheckbox = tvchannelAttributeMapping.AttributeControlType == AttributeControlType.Checkboxes ||
                                     tvchannelAttributeMapping.AttributeControlType ==
                                     AttributeControlType.ReadonlyCheckboxes;

                    var currentAttributesXml = new List<string>();

                    if (isCheckbox)
                    {
                        //add several values attribute types (checkboxes)

                        //checkboxes could have several values ticked
                        foreach (var oldXml in attributesXml.Any() ? attributesXml : new List<string> { string.Empty })
                        {
                            foreach (var checkboxCombination in CreateCombination(attributeValues))
                            {
                                var newXml = oldXml;
                                foreach (var checkboxValue in checkboxCombination) 
                                    newXml = AddTvChannelAttribute(newXml, tvchannelAttributeMapping, checkboxValue.Id.ToString());

                                if (!string.IsNullOrEmpty(newXml)) 
                                    currentAttributesXml.Add(newXml);
                            }
                        }
                    }
                    else
                    {
                        //add one value attribute types (dropdownlist, radiobutton, color squares)

                        foreach (var oldXml in attributesXml.Any() ? attributesXml : new List<string> { string.Empty })
                        {
                            currentAttributesXml.AddRange(attributeValues.Select(attributeValue =>
                                AddTvChannelAttribute(oldXml, tvchannelAttributeMapping, attributeValue.Id.ToString())));
                        }
                    }

                    attributesXml.Clear();
                    attributesXml.AddRange(currentAttributesXml);
                }

                allAttributesXml.AddRange(attributesXml);
            }

            //validate conditional attributes (if specified)
            //minor workaround:
            //once it's done (validation), then we could have some duplicated combinations in result
            //we don't remove them here (for performance optimization) because anyway it'll be done in the "GenerateAllAttributeCombinations" method of TvChannelController
            for (var i = 0; i < allAttributesXml.Count; i++)
            {
                var attributesXml = allAttributesXml[i];
                foreach (var attribute in allTvChannelAttributeMappings)
                {
                    var conditionMet = await IsConditionMetAsync(attribute, attributesXml);
                    if (conditionMet.HasValue && !conditionMet.Value) 
                        allAttributesXml[i] = RemoveTvChannelAttribute(attributesXml, attribute);
                }
            }

            return allAttributesXml;
        }

        /// <summary>
        /// Parse a user entered price of the tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user entered price of the tvchannel
        /// </returns>
        public virtual async Task<decimal> ParseUserEnteredPriceAsync(TvChannel tvchannel, IFormCollection form)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var userEnteredPriceConverted = decimal.Zero;
            if (tvchannel.UserEntersPrice)
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{tvchannel.Id}.UserEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out var userEnteredPrice))
                            userEnteredPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(userEnteredPrice, await _workContext.GetWorkingCurrencyAsync());
                        break;
                    }
                }

            return userEnteredPriceConverted;
        }

        /// <summary>
        /// Parse a entered quantity of the tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <returns>User entered price of the tvchannel</returns>
        public virtual int ParseEnteredQuantity(TvChannel tvchannel, IFormCollection form)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var quantity = 1;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{tvchannel.Id}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    break;
                }

            return quantity;
        }

        /// <summary>
        /// Parse tvchannel rental dates on the tvchannel details page
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        public virtual void ParseRentalDates(TvChannel tvchannel, IFormCollection form, out DateTime? startDate, out DateTime? endDate)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            startDate = null;
            endDate = null;

            if (tvchannel.IsRental)
            {
                var ctrlStartDate = form[$"rental_start_date_{tvchannel.Id}"];
                var ctrlEndDate = form[$"rental_end_date_{tvchannel.Id}"];
                try
                {
                    startDate = DateTime.ParseExact(ctrlStartDate, 
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, 
                        CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(ctrlEndDate, 
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, 
                        CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Get tvchannel attributes from the passed form
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="form">Form values</param>
        /// <param name="errors">Errors</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes in XML format
        /// </returns>
        public virtual async Task<string> ParseTvChannelAttributesAsync(TvChannel tvchannel, IFormCollection form, List<string> errors)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            //tvchannel attributes
            var attributesXml = await GetTvChannelAttributesXmlAsync(tvchannel, form, errors);

            //gift cards
            AddGiftCardsAttributesXml(tvchannel, form, ref attributesXml);

            return attributesXml;
        }

        #endregion

        #region Gift card attributes

        /// <summary>
        /// Add gift card attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        /// <returns>Attributes</returns>
        public string AddGiftCardAttribute(string attributesXml, string recipientName,
            string recipientEmail, string senderName, string senderEmail, string giftCardMessage)
        {
            var result = string.Empty;
            try
            {
                recipientName = recipientName.Trim();
                recipientEmail = recipientEmail.Trim();
                senderName = senderName.Trim();
                senderEmail = senderEmail.Trim();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(attributesXml);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                var giftCardElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo");
                if (giftCardElement == null)
                {
                    giftCardElement = xmlDoc.CreateElement("GiftCardInfo");
                    rootElement.AppendChild(giftCardElement);
                }

                var recipientNameElement = xmlDoc.CreateElement("RecipientName");
                recipientNameElement.InnerText = recipientName;
                giftCardElement.AppendChild(recipientNameElement);

                var recipientEmailElement = xmlDoc.CreateElement("RecipientEmail");
                recipientEmailElement.InnerText = recipientEmail;
                giftCardElement.AppendChild(recipientEmailElement);

                var senderNameElement = xmlDoc.CreateElement("SenderName");
                senderNameElement.InnerText = senderName;
                giftCardElement.AppendChild(senderNameElement);

                var senderEmailElement = xmlDoc.CreateElement("SenderEmail");
                senderEmailElement.InnerText = senderEmail;
                giftCardElement.AppendChild(senderEmailElement);

                var messageElement = xmlDoc.CreateElement("Message");
                messageElement.InnerText = giftCardMessage;
                giftCardElement.AppendChild(messageElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Get gift card attributes
        /// </summary>
        /// <param name="attributesXml">Attributes</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        public void GetGiftCardAttribute(string attributesXml, out string recipientName,
            out string recipientEmail, out string senderName,
            out string senderEmail, out string giftCardMessage)
        {
            recipientName = string.Empty;
            recipientEmail = string.Empty;
            senderName = string.Empty;
            senderEmail = string.Empty;
            giftCardMessage = string.Empty;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var recipientNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientName");
                var recipientEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientEmail");
                var senderNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderName");
                var senderEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderEmail");
                var messageElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/Message");

                if (recipientNameElement != null)
                    recipientName = recipientNameElement.InnerText;
                if (recipientEmailElement != null)
                    recipientEmail = recipientEmailElement.InnerText;
                if (senderNameElement != null)
                    senderName = senderNameElement.InnerText;
                if (senderEmailElement != null)
                    senderEmail = senderEmailElement.InnerText;
                if (messageElement != null)
                    giftCardMessage = messageElement.InnerText;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
        }

        #endregion

        #region Properties

        protected override string RootElementName { get; set; } = "Attributes";

        protected override string ChildElementName { get; set; } = "TvChannelAttribute";

        #endregion
    }
}