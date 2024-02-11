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
    /// Compare tvchannels service
    /// </summary>
    public partial class CompareTvChannelsService : ICompareTvChannelsService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CookieSettings _cookieSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITvChannelService _tvchannelService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CompareTvChannelsService(CatalogSettings catalogSettings,
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
        /// Get a list of identifier of compared tvchannels
        /// </summary>
        /// <returns>List of identifier</returns>
        protected virtual List<int> GetComparedTvChannelIds()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            var cookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.ComparedTvChannelsCookie}";
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var tvchannelIdsCookie) || string.IsNullOrEmpty(tvchannelIdsCookie))
                return new List<int>();

            //get array of string tvchannel identifiers from cookie
            var tvchannelIds = tvchannelIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int tvchannel identifiers
            return tvchannelIds.Select(int.Parse).Distinct().ToList();
        }

        /// <summary>
        /// Add cookie value for the compared tvchannels
        /// </summary>
        /// <param name="comparedTvChannelIds">Collection of compared tvchannels identifiers</param>
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
        /// Clears a "compare tvchannels" list
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
        /// Gets a "compare tvchannels" list
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the "Compare tvchannels" list
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetComparedTvChannelsAsync()
        {
            //get list of compared tvchannel identifiers
            var tvchannelIds = GetComparedTvChannelIds();

            //return list of tvchannel
            return (await _tvchannelService.GetTvChannelsByIdsAsync(tvchannelIds.ToArray()))
                .Where(tvchannel => tvchannel.Published && !tvchannel.Deleted).ToList();
        }

        /// <summary>
        /// Removes a tvchannel from a "compare tvchannels" list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task RemoveTvChannelFromCompareListAsync(int tvchannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return Task.CompletedTask;

            //get list of compared tvchannel identifiers
            var comparedTvChannelIds = GetComparedTvChannelIds();

            //whether tvchannel identifier to remove exists
            if (!comparedTvChannelIds.Contains(tvchannelId))
                return Task.CompletedTask;

            //it exists, so remove it from list
            comparedTvChannelIds.Remove(tvchannelId);

            //set cookie
            AddCompareTvChannelsCookie(comparedTvChannelIds);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a tvchannel to a "compare tvchannels" list
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task AddTvChannelToCompareListAsync(int tvchannelId)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return Task.CompletedTask;

            //get list of compared tvchannel identifiers
            var comparedTvChannelIds = GetComparedTvChannelIds();

            //whether tvchannel identifier to add already exist
            if (!comparedTvChannelIds.Contains(tvchannelId))
                comparedTvChannelIds.Insert(0, tvchannelId);

            //limit list based on the allowed number of tvchannels to be compared
            comparedTvChannelIds = comparedTvChannelIds.Take(_catalogSettings.CompareTvChannelsNumber).ToList();

            //set cookie
            AddCompareTvChannelsCookie(comparedTvChannelIds);

            return Task.CompletedTask;
        }

        #endregion
    }
}