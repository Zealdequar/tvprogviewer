using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Copy tvchannel service
    /// </summary>
    public partial interface ICopyTvChannelService
    {
        /// <summary>
        /// Create a copy of tvchannel with all depended data
        /// </summary>
        /// <param name="tvchannel">The tvchannel to copy</param>
        /// <param name="newName">The name of tvchannel duplicate</param>
        /// <param name="isPublished">A value indicating whether the tvchannel duplicate should be published</param>
        /// <param name="copyMultimedia">A value indicating whether the tvchannel images and videos should be copied</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether the copy associated tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel copy
        /// </returns>
        Task<TvChannel> CopyTvChannelAsync(TvChannel tvchannel, string newName,
            bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedTvChannels = true);
    }
}
