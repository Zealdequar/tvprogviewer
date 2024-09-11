using Microsoft.AspNetCore.Http;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to get all categories
    /// </summary>
    public class GetCategoriesRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "organizations/self/categories/v2";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}