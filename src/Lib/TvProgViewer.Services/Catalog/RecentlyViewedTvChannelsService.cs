using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Security;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Recently viewed tvChannels service
    /// </summary>
    public partial class RecentlyViewedTvChannelsService : IRecentlyViewedTvChannelsService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CookieSettings _cookieSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITvChannelService _tvChannelService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public RecentlyViewedTvChannelsService(CatalogSettings catalogSettings,
            CookieSettings cookieSettings,
            IHttpContextAccessor httpContextAccessor,
            ITvChannelService tvChannelService,
            IWebHelper webHelper)
        {
            _catalogSettings = catalogSettings;
            _cookieSettings = cookieSettings;
            _httpContextAccessor = httpContextAccessor;
            _tvChannelService = tvChannelService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a list of identifier of recently viewed tvChannels
        /// </summary>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedTvChannelsIds()
        {
            return GetRecentlyViewedTvChannelsIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a list of identifier of recently viewed tvChannels
        /// </summary>
        /// <param name="number">Number of tvChannels to load</param>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedTvChannelsIds(int number)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.RecentlyViewedTvChannelsCookie}";
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var tvChannelIdsCookie) || string.IsNullOrEmpty(tvChannelIdsCookie))
                return new List<int>();

            //get array of string tvChannel identifiers from cookie
            var tvChannelIds = tvChannelIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int tvChannel identifiers
            return tvChannelIds.Select(int.Parse).Distinct().Take(number).ToList();
        }

        /// <summary>
        /// Add cookie value for the recently viewed tvChannels
        /// </summary>
        /// <param name="recentlyViewedTvChannelIds">Collection of the recently viewed tvChannels identifiers</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual Task AddRecentlyViewedTvChannelsCookieAsync(IEnumerable<int> recentlyViewedTvChannelIds)
        {
            //delete current cookie if exists
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.RecentlyViewedTvChannelsCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //create cookie value
            var tvChannelIdsCookie = string.Join(",", recentlyViewedTvChannelIds);

            //create cookie options 
            var cookieExpires = _cookieSettings.RecentlyViewedTvChannelsCookieExpires;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(cookieExpires),
                HttpOnly = true,
                Secure = _webHelper.IsCurrentConnectionSecured()
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, tvChannelIdsCookie, cookieOptions);

            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a "recently viewed tvChannels" list
        /// </summary>
        /// <param name="number">Number of tvChannels to load</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the "recently viewed tvChannels" list
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetRecentlyViewedTvChannelsAsync(int number)
        {
            //get list of recently viewed tvChannel identifiers
            var tvChannelIds = GetRecentlyViewedTvChannelsIds(number);

            //return list of tvChannel
            return (await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelIds.ToArray()))
                .Where(tvChannel => tvChannel.Published && !tvChannel.Deleted).ToList();
        }

        /// <summary>
        /// Adds a tvChannel to a recently viewed tvChannels list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddTvChannelToRecentlyViewedListAsync(int tvChannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //whether recently viewed tvChannels is enabled
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return;

            //get list of recently viewed tvChannel identifiers
            var tvChannelIds = GetRecentlyViewedTvChannelsIds();

            //whether tvChannel identifier to add already exist
            if (!tvChannelIds.Contains(tvChannelId))
                tvChannelIds.Insert(0, tvChannelId);

            //limit list based on the allowed number of the recently viewed tvChannels
            tvChannelIds = tvChannelIds.Take(_catalogSettings.RecentlyViewedTvChannelsNumber).ToList();

            //set cookie
            await AddRecentlyViewedTvChannelsCookieAsync(tvChannelIds);
        }

        #endregion
    }
}