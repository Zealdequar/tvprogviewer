using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Media;

namespace TvProgViewer.Services.Media
{
    /// <summary>
    /// Video service interface
    /// </summary>
    public partial interface IVideoService
    {
        /// <summary>
        /// Gets a video
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        Task<Video> GetVideoByIdAsync(int videoId);

        /// <summary>
        /// Gets videos by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the videos
        /// </returns>
        Task<IList<Video>> GetVideosByTvChannelIdAsync(int tvChannelId);

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        Task<Video> InsertVideoAsync(Video video);

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        Task<Video> UpdateVideoAsync(Video video);

        /// <summary>
        /// Deletes a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteVideoAsync(Video video);
    }
}
