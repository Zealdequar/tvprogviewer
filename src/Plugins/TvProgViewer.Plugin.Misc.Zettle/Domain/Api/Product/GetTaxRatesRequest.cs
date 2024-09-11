using Microsoft.AspNetCore.Http;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents request to get all tax rates
    /// </summary>
    public class GetTaxRatesRequest : TvChannelApiRequest
    {
        /// <summary>
        /// Gets the request path
        /// </summary>
        public override string Path => "v1/taxes";

        /// <summary>
        /// Gets the request method
        /// </summary>
        public override string Method => HttpMethods.Get;
    }
}