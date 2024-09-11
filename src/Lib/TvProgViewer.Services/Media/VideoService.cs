using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;

namespace TvProgViewer.Services.Media
{
    /// <summary>
    /// Video service
    /// </summary>
    public partial class VideoService : IVideoService
    {
        #region Fields

        private readonly IRepository<TvChannelVideo> _tvChannelVideoRepository;
        private readonly IRepository<Video> _videoRepository;

        #endregion

        #region Ctor

        public VideoService(IRepository<TvChannelVideo> tvChannelVideoRepository,
            IRepository<Video> videoRepository)
        {
            _tvChannelVideoRepository = tvChannelVideoRepository;
            _videoRepository = videoRepository;
        }

        #endregion

        #region CRUD methods

        /// <summary>
        /// Gets a video
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> GetVideoByIdAsync(int videoId)
        {
            return await _videoRepository.GetByIdAsync(videoId, cache => default);
        }

        /// <summary>
        /// Gets videos by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the videos
        /// </returns>
        public virtual async Task<IList<Video>> GetVideosByTvChannelIdAsync(int tvChannelId)
        {
            if (tvChannelId == 0)
                return new List<Video>();

            var query = from v in _videoRepository.Table
                        join pv in _tvChannelVideoRepository.Table on v.Id equals pv.VideoId
                        orderby pv.DisplayOrder, pv.Id
                        where pv.TvChannelId == tvChannelId
                        select v;

            var videos = await query.ToListAsync();

            return videos;
        }

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> InsertVideoAsync(Video video)
        {
            await _videoRepository.InsertAsync(video);
            return video;
        }

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> UpdateVideoAsync(Video video)
        {
            await _videoRepository.UpdateAsync(video);
            
            return video;
        }

        /// <summary>
        /// Deletes a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteVideoAsync(Video video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            await _videoRepository.DeleteAsync(video);
        }

        #endregion
    }
}
