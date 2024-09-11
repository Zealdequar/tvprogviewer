namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents base request to TvChannel API
    /// </summary>
    public abstract class TvChannelApiRequest : ApiRequest, IAuthorizedRequest
    {
        /// <summary>
        /// Gets the request base URL
        /// </summary>
        public override string BaseUrl => "https://tvChannels.izettle.com/";
    }
}