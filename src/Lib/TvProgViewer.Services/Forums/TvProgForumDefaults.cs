using TvProgViewer.Core.Caching;

namespace TvProgViewer.Services.Forums
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class TvProgForumDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        public static CacheKey ForumByForumGroupCacheKey => new("TvProg.forum.byforumgroup.{0}");

        #endregion
    }
}