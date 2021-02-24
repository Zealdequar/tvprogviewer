using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User attribute parser interface
    /// </summary>
    public partial interface IUserAttributeParser
    {
        /// <summary>
        /// Gets selected user attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected user attributes</returns>
        Task<IList<UserAttribute>> ParseUserAttributesAsync(string attributesXml);

        /// <summary>
        /// Get user attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>User attribute values</returns>
        Task<IList<UserAttributeValue>> ParseUserAttributeValuesAsync(string attributesXml);

        //TODO: migrate to an extension method
        /// <summary>
        /// Gets selected user attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userAttributeId">User attribute identifier</param>
        /// <returns>User attribute value</returns>
        IList<string> ParseValues(string attributesXml, int userAttributeId);

        //TODO: migrate to an extension method
        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">User attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddUserAttribute(string attributesXml, UserAttribute ca, string value);

        /// <summary>
        /// Validates user attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        Task<IList<string>> GetAttributeWarningsAsync(string attributesXml);
    }
}
