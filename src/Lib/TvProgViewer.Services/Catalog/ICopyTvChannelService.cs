using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Copy tvChannel service
    /// </summary>
    public partial interface ICopyTvChannelService
    {
        /// <summary>
        /// Create a copy of tvChannel with all depended data
        /// </summary>
        /// <param name="tvChannel">The tvChannel to copy</param>
        /// <param name="newName">The name of tvChannel duplicate</param>
        /// <param name="isPublished">A value indicating whether the tvChannel duplicate should be published</param>
        /// <param name="copyMultimedia">A value indicating whether the tvChannel images and videos should be copied</param>
        /// <param name="copyAssociatedTvChannels">A value indicating whether the copy associated tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel copy
        /// </returns>
        Task<TvChannel> CopyTvChannelAsync(TvChannel tvChannel, string newName,
            bool isPublished = true, bool copyMultimedia = true, bool copyAssociatedTvChannels = true);
    }
}
