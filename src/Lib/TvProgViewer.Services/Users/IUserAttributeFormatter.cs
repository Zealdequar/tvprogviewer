﻿using System.Threading.Tasks;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// User attribute helper
    /// </summary>
    public partial interface IUserAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes
        /// </returns>
        Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true);
    }
}
