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
    /// Compare tvChannels service
    /// </summary>
    public partial class CompareTvChannelsService : ICompareTvChannelsService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CookieSettings _cookieSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITvChannelService _tvChannelService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CompareTvChannelsService(CatalogSettings catalogSettings,
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
        /// Get a list of identifier of compared tvChannels
        /// </summary>
        /// <returns>List of identifier</returns>
        protected virtual List<int> GetComparedTvChannelIds()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.ComparedTvChannelsCookie}";
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var tvChannelIdsCookie) || string.IsNullOrEmpty(tvChannelIdsCookie))
                return new List<int>();

            //get array of string tvChannel identifiers from cookie
            var tvChannelIds = tvChannelIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int tvChannel identifiers
            return tvChannelIds.Select(int.Parse).Distinct().ToList();
        }

        /// <summary>
        /// Add cookie value for the compared tvChannels
        /// </summary>
        /// <param name="comparedTvChannelIds">Collection of compared tvChannels identifiers</param>
        protected virtual void AddCompareTvChannelsCookie(IEnumerable<int> comparedTvChannelIds)
        {
            //delete current cookie if exists
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.ComparedTvChannelsCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //create cookie value
            var comparedTvChannelIdsCookie = string.Join(",", comparedTvChannelIds);

            //create cookie options 
            var cookieExpires = _cookieSettings.CompareTvChannelsCookieExpires;
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(cookieExpires),
                HttpOnly = true,
                Secure =  _webHelper.IsCurrentConnectionSecured()
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, comparedTvChannelIdsCookie, cookieOptions);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare tvChannels" list
        /// </summary>
        public virtual void ClearCompareTvChannels()
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //sets an expired cookie
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.ComparedTvChannelsCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
        }

        /// <summary>
        /// Gets a "compare tvChannels" list
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the "Compare tvChannels" list
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetComparedTvChannelsAsync()
        {
            //get list of compared tvChannel identifiers
            var tvChannelIds = GetComparedTvChannelIds();

            //return list of tvChannel
            return (await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelIds.ToArray()))
                .Where(tvChannel => tvChannel.Published && !tvChannel.Deleted).ToList();
        }

        /// <summary>
        /// Removes a tvChannel from a "compare tvChannels" list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual Task RemoveTvChannelFromCompareListAsync(int tvChannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return Task.CompletedTask;

            //get list of compared tvChannel identifiers
            var comparedTvChannelIds = GetComparedTvChannelIds();

            //whether tvChannel identifier to remove exists
            if (!comparedTvChannelIds.Contains(tvChannelId))
                return Task.CompletedTask;

            //it exists, so remove it from list
            comparedTvChannelIds.Remove(tvChannelId);

            //set cookie
            AddCompareTvChannelsCookie(comparedTvChannelIds);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a tvChannel to a "compare tvChannels" list
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual Task AddTvChannelToCompareListAsync(int tvChannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return Task.CompletedTask;

            //get list of compared tvChannel identifiers
            var comparedTvChannelIds = GetComparedTvChannelIds();

            //whether tvChannel identifier to add already exist
            if (!comparedTvChannelIds.Contains(tvChannelId))
                comparedTvChannelIds.Insert(0, tvChannelId);

            //limit list based on the allowed number of tvChannels to be compared
            comparedTvChannelIds = comparedTvChannelIds.Take(_catalogSettings.CompareTvChannelsNumber).ToList();

            //set cookie
            AddCompareTvChannelsCookie(comparedTvChannelIds);

            return Task.CompletedTask;
        }

        #endregion
    }
}