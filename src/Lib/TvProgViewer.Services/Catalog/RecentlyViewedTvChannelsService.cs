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
    /// Recently viewed tvchannels service
    /// </summary>
    public partial class RecentlyViewedTvChannelsService : IRecentlyViewedTvChannelsService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CookieSettings _cookieSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITvChannelService _tvchannelService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public RecentlyViewedTvChannelsService(CatalogSettings catalogSettings,
            CookieSettings cookieSettings,
            IHttpContextAccessor httpContextAccessor,
            ITvChannelService tvchannelService,
            IWebHelper webHelper)
        {
            _catalogSettings = catalogSettings;
            _cookieSettings = cookieSettings;
            _httpContextAccessor = httpContextAccessor;
            _tvchannelService = tvchannelService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a list of identifier of recently viewed tvchannels
        /// </summary>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedTvChannelsIds()
        {
            return GetRecentlyViewedTvChannelsIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a list of identifier of recently viewed tvchannels
        /// </summary>
        /// <param name="number">Number of tvchannels to load</param>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedTvChannelsIds(int number)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.RecentlyViewedTvChannelsCookie}";
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var tvchannelIdsCookie) || string.IsNullOrEmpty(tvchannelIdsCookie))
                return new List<int>();

            //get array of string tvchannel identifiers from cookie
            var tvchannelIds = tvchannelIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int tvchannel identifiers
            return tvchannelIds.Select(int.Parse).Distinct().Take(number).ToList();
        }

        /// <summary>
        /// Add cookie value for the recently viewed tvchannels
        /// </summary>
        /// <param name="recentlyViewedTvChannelIds">Collection of the recently viewed tvchannels identifiers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual Task AddRecentlyViewedTvChannelsCookieAsync(IEnumerable<int> recentlyViewedTvChannelIds)
        {
            //delete current cookie if exists
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.RecentlyViewedTvChannelsCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //create cookie value
            var tvchannelIdsCookie = string.Join(",", recentlyViewedTvChannelIds);

            //create cookie options 
            var cookieExpires = _cookieSettings.RecentlyViewedTvChannelsCookieExpires;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(cookieExpires),
                HttpOnly = true,
                Secure = _webHelper.IsCurrentConnectionSecured()
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, tvchannelIdsCookie, cookieOptions);

            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a "recently viewed tvchannels" list
        /// </summary>
        /// <param name="number">Number of tvchannels to load</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "recently viewed tvchannels" list
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetRecentlyViewedTvChannelsAsync(int number)
        {
            //get list of recently viewed tvchannel identifiers
            var tvchannelIds = GetRecentlyViewedTvChannelsIds(number);

            //return list of tvchannel
            return (await _tvchannelService.GetTvChannelsByIdsAsync(tvchannelIds.ToArray()))
                .Where(tvchannel => tvchannel.Published && !tvchannel.Deleted).ToList();
        }

        /// <summary>
        /// Adds a tvchannel to a recently viewed tvchannels list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddTvChannelToRecentlyViewedListAsync(int tvchannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //whether recently viewed tvchannels is enabled
            if (!_catalogSettings.RecentlyViewedTvChannelsEnabled)
                return;

            //get list of recently viewed tvchannel identifiers
            var tvchannelIds = GetRecentlyViewedTvChannelsIds();

            //whether tvchannel identifier to add already exist
            if (!tvchannelIds.Contains(tvchannelId))
                tvchannelIds.Insert(0, tvchannelId);

            //limit list based on the allowed number of the recently viewed tvchannels
            tvchannelIds = tvchannelIds.Take(_catalogSettings.RecentlyViewedTvChannelsNumber).ToList();

            //set cookie
            await AddRecentlyViewedTvChannelsCookieAsync(tvchannelIds);
        }

        #endregion
    }
}