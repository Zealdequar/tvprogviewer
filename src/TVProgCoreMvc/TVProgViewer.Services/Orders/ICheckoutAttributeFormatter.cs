using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Orders
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial interface ICheckoutAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="user">User</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperlink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        Task<string> FormatAttributesAsync(string attributesXml,
            User user,
            string separator = "<br />",
            bool htmlEncode = true,
            bool renderPrices = true,
            bool allowHyperlinks = true);
    }
}
