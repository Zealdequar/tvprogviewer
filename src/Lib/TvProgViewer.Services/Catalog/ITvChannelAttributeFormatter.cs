﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute formatter interface
    /// </summary>
    public partial interface ITvChannelAttributeFormatter
    {
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes
        /// </returns>
        Task<string> FormatAttributesAsync(TvChannel tvChannel, string attributesXml);

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderTvChannelAttributes">A value indicating whether to render tvChannel attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the attributes
        /// </returns>
        Task<string> FormatAttributesAsync(TvChannel tvChannel, string attributesXml,
            User user, Store store, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderTvChannelAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true);
    }
}
