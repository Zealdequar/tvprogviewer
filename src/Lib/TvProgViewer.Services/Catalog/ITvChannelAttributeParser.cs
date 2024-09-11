using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute parser interface
    /// </summary>
    public partial interface ITvChannelAttributeParser
    {
        #region TvChannel attributes

        /// <summary>
        /// Gets selected tvChannel attribute mappings
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the selected tvChannel attribute mappings
        /// </returns>
        Task<IList<TvChannelAttributeMapping>> ParseTvChannelAttributeMappingsAsync(string attributesXml);

        /// <summary>
        /// Get tvChannel attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvChannelAttributeMappingId">TvChannel attribute mapping identifier; pass 0 to load all values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute values
        /// </returns>
        Task<IList<TvChannelAttributeValue>> ParseTvChannelAttributeValuesAsync(string attributesXml, int tvChannelAttributeMappingId = 0);

        /// <summary>
        /// Gets selected tvChannel attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvChannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>TvChannel attribute values</returns>
        IList<string> ParseValues(string attributesXml, int tvChannelAttributeMappingId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="value">Value</param>
        /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToTvChannel to specify the quantity entered by the user)</param>
        /// <returns>Updated result (XML format)</returns>
        string AddTvChannelAttribute(string attributesXml, TvChannelAttributeMapping tvChannelAttributeMapping, string value, int? quantity = null);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveTvChannelAttribute(string attributesXml, TvChannelAttributeMapping tvChannelAttributeMapping);

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributesXml1">The attributes of the first tvChannel</param>
        /// <param name="attributesXml2">The attributes of the second tvChannel</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreQuantity">A value indicating whether we should ignore the quantity of attribute value entered by the user</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> AreTvChannelAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="pam">TvChannel attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool?> IsConditionMetAsync(TvChannelAttributeMapping pam, string selectedAttributesXml);

        /// <summary>
        /// Finds a tvChannel attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the found tvChannel attribute combination
        /// </returns>
        Task<TvChannelAttributeCombination> FindTvChannelAttributeCombinationAsync(TvChannel tvChannel,
            string attributesXml, bool ignoreNonCombinableAttributes = true);

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="allowedAttributeIds">List of allowed attribute identifiers. If null or empty then all attributes would be used.</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attribute combinations in XML format
        /// </returns>
        Task<IList<string>> GenerateAllCombinationsAsync(TvChannel tvChannel, bool ignoreNonCombinableAttributes = false, IList<int> allowedAttributeIds = null);

        /// <summary>
        /// Parse a user entered price of the tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user entered price of the tvChannel
        /// </returns>
        Task<decimal> ParseUserEnteredPriceAsync(TvChannel tvChannel, IFormCollection form);

        /// <summary>
        /// Parse a entered quantity of the tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <returns>User entered price of the tvChannel</returns>
        int ParseEnteredQuantity(TvChannel tvChannel, IFormCollection form);

        /// <summary>
        /// Parse tvChannel rental dates on the tvChannel details page
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        void ParseRentalDates(TvChannel tvChannel, IFormCollection form, out DateTime? startDate, out DateTime? endDate);

        /// <summary>
        /// Get tvChannel attributes from the passed form
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="form">Form values</param>
        /// <param name="errors">Errors</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes in XML format
        /// </returns>
        Task<string> ParseTvChannelAttributesAsync(TvChannel tvChannel, IFormCollection form, List<string> errors);

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
        string AddGiftCardAttribute(string attributesXml, string recipientName,
            string recipientEmail, string senderName, string senderEmail, string giftCardMessage);

        /// <summary>
        /// Get gift card attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        void GetGiftCardAttribute(string attributesXml, out string recipientName,
            out string recipientEmail, out string senderName,
            out string senderEmail, out string giftCardMessage);

        #endregion
    }
}
