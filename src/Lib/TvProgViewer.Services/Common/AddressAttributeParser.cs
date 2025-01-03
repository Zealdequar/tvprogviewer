﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Localization;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Address attribute parser
    /// </summary>
    public partial class AddressAttributeParser : IAddressAttributeParser
    {
        #region Fields

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public AddressAttributeParser(IAddressAttributeService addressAttributeService,
            ILocalizationService localizationService)
        {
            _addressAttributeService = addressAttributeService;
            _localizationService = localizationService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets selected address attribute identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected address attribute identifiers</returns>
        protected virtual IList<int> ParseAddressAttributeIds(string attributesXml)
        {
            var ids = new List<int>();
            if (string.IsNullOrEmpty(attributesXml))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/AddressAttribute"))
                {
                    if (node.Attributes?["ID"] == null) 
                        continue;

                    var str1 = node.Attributes["ID"].InnerText.Trim();
                    if (int.TryParse(str1, out var id))
                    {
                        ids.Add(id);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return ids;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets selected address attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the selected address attributes
        /// </returns>
        public virtual async Task<IList<AddressAttribute>> ParseAddressAttributesAsync(string attributesXml)
        {
            var result = new List<AddressAttribute>();
            if (string.IsNullOrEmpty(attributesXml))
                return result;

            var ids = ParseAddressAttributeIds(attributesXml);
            foreach (var id in ids)
            {
                var attribute = await _addressAttributeService.GetAddressAttributeByIdAsync(id);
                if (attribute != null) 
                    result.Add(attribute);
            }

            return result;
        }

        /// <summary>
        /// Get address attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address attribute values
        /// </returns>
        public virtual async Task<IList<AddressAttributeValue>> ParseAddressAttributeValuesAsync(string attributesXml)
        {
            var values = new List<AddressAttributeValue>();
            if (string.IsNullOrEmpty(attributesXml))
                return values;

            var attributes = await ParseAddressAttributesAsync(attributesXml);
            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                var valuesStr = ParseValues(attributesXml, attribute.Id);
                foreach (var valueStr in valuesStr)
                {
                    if (string.IsNullOrEmpty(valueStr)) 
                        continue;

                    if (!int.TryParse(valueStr, out var id))
                        continue;

                    var value = await _addressAttributeService.GetAddressAttributeValueByIdAsync(id);
                    if (value != null)
                        values.Add(value);
                }
            }

            return values;
        }

        /// <summary>
        /// Gets selected address attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute value</returns>
        public virtual IList<string> ParseValues(string attributesXml, int addressAttributeId)
        {
            var selectedAddressAttributeValues = new List<string>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedAddressAttributeValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/AddressAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null) 
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id)) 
                        continue;

                    if (id != addressAttributeId) 
                        continue;

                    var nodeList2 = node1.SelectNodes(@"AddressAttributeValue/Value");
                    foreach (XmlNode node2 in nodeList2)
                    {
                        var value = node2.InnerText.Trim();
                        selectedAddressAttributeValues.Add(value);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return selectedAddressAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Address attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddAddressAttribute(string attributesXml, AddressAttribute attribute, string value)
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
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/AddressAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null) 
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id)) 
                        continue;

                    if (id != attribute.Id) 
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("AddressAttribute");
                    attributeElement.SetAttribute("ID", attribute.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("AddressAttributeValue");
                attributeElement.AppendChild(attributeValueElement);

                var attributeValueValueElement = xmlDoc.CreateElement("Value");
                attributeValueValueElement.InnerText = value;
                attributeValueElement.AppendChild(attributeValueValueElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Validates address attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetAttributeWarningsAsync(string attributesXml)
        {
            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = await ParseAddressAttributesAsync(attributesXml);

            //validate required address attributes (whether they're chosen/selected/entered)
            var attributes2 = await _addressAttributeService.GetAllAddressAttributesAsync();
            foreach (var a2 in attributes2)
            {
                if (!a2.IsRequired) 
                    continue;

                var found = false;
                //selected address attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id) 
                        continue;

                    var valuesStr = ParseValues(attributesXml, a1.Id);

                    found = valuesStr.Any(str1 => !string.IsNullOrEmpty(str1.Trim()));
                }
                
                if (found) 
                    continue;

                //if not found
                var notFoundWarning = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(a2, a => a.Name));

                warnings.Add(notFoundWarning);
            }

            return warnings;
        }

        /// <summary>
        /// Get custom address attributes from the passed form
        /// </summary>
        /// <param name="form">Form values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes in XML format
        /// </returns>
        public virtual async Task<string> ParseCustomAddressAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;

            foreach (var attribute in await _addressAttributeService.GetAllAddressAttributesAsync())
            {
                var controlId = string.Format(TvProgCommonDefaults.AddressAttributeControlName, attribute.Id);
                var attributeValues = form[controlId];
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                            attributesXml = AddAddressAttribute(attributesXml, attribute, value.ToString());
                        break;

                    case AttributeControlType.Checkboxes:
                        foreach (var attributeValue in attributeValues.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(attributeValue, out value) && value > 0)
                                attributesXml = AddAddressAttribute(attributesXml, attribute, value.ToString());
                        }

                        break;

                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var addressAttributeValues = await _addressAttributeService.GetAddressAttributeValuesAsync(attribute.Id);
                        foreach (var addressAttributeValue in addressAttributeValues)
                        {
                            if (addressAttributeValue.IsPreSelected)
                                attributesXml = AddAddressAttribute(attributesXml, attribute, addressAttributeValue.Id.ToString());
                        }

                        break;

                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        if (!StringValues.IsNullOrEmpty(attributeValues))
                            attributesXml = AddAddressAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                        break;

                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        #endregion
    }
}