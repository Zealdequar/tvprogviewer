using Microsoft.AspNetCore.Http;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.Product
{
    /// <summary>
    /// Represents request to get all discounts
    /// </summary>
    public class GetDiscountsRequest : ProductApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "organizations/self/discounts";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}