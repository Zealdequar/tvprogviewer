﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Profile;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the profile model factory
    /// </summary>
    public partial class ProfileModelFactory : IProfileModelFactory
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ProfileModelFactory(UserSettings userSettings,
            ForumSettings forumSettings,
            ICountryService countryService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _userSettings = userSettings;
            _forumSettings = forumSettings;
            _countryService = countryService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile index model
        /// </returns>
        public virtual async Task<ProfileIndexModel> PrepareProfileIndexModelAsync(User user, int? page)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var pagingPosts = false;
            var postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = await _userService.FormatUsernameAsync(user);
            var title = string.Format(await _localizationService.GetResourceAsync("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                UserProfileId = user.Id,
                ForumsEnabled = _forumSettings.ForumsEnabled
            };
            return model;
        }

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile info model
        /// </returns>
        public virtual async Task<ProfileInfoModel> PrepareProfileInfoModelAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //avatar
            var avatarUrl = "";
            if (_userSettings.AllowUsersToUploadAvatars)
            {
                avatarUrl = await _pictureService.GetPictureUrlAsync(
                 await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute),
                 _mediaSettings.AvatarPictureSize,
                 _userSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            var locationEnabled = false;
            var location = string.Empty;
            if (_userSettings.ShowUsersLocation)
            {
                locationEnabled = true;

                var countryId = user.CountryId;
                var country = await _countryService.GetCountryByIdAsync(countryId);
                if (country != null)
                {
                    location = await _localizationService.GetLocalizedAsync(country, x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }

            //private message
            var pmEnabled = _forumSettings.AllowPrivateMessages && !await _userService.IsGuestAsync(user);

            //total forum posts
            var totalPostsEnabled = false;
            var totalPosts = 0;
            if (_forumSettings.ForumsEnabled && _forumSettings.ShowUsersPostCount)
            {
                totalPostsEnabled = true;
                totalPosts = await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.ForumPostCountAttribute);
            }

            //registration date
            var joinDateEnabled = false;
            var joinDate = string.Empty;

            if (_userSettings.ShowUsersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = (await _dateTimeHelper.ConvertToUserTimeAsync(user.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
            }

            //birth date
            var dateOfBirthEnabled = false;
            var dateOfBirth = string.Empty;
            if (_userSettings.BirthDateEnabled)
            {
                if (user.BirthDate.HasValue)
                {
                    dateOfBirthEnabled = true;
                    dateOfBirth = user.BirthDate.Value.ToString("D");
                }
            }

            var model = new ProfileInfoModel
            {
                UserProfileId = user.Id,
                AvatarUrl = avatarUrl,
                LocationEnabled = locationEnabled,
                Location = location,
                PMEnabled = pmEnabled,
                TotalPostsEnabled = totalPostsEnabled,
                TotalPosts = totalPosts.ToString(),
                JoinDateEnabled = joinDateEnabled,
                JoinDate = joinDate,
                BirthDateEnabled = dateOfBirthEnabled,
                BirthDate = dateOfBirth,
            };

            return model;
        }

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the profile posts model
        /// </returns>
        public virtual async Task<ProfilePostsModel> PrepareProfilePostsModelAsync(User user, int page)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.LatestUserPostsPageSize;

            var list = await _forumService.GetAllPostsAsync(0, user.Id, string.Empty, false, page, pageSize);

            var latestPosts = new List<PostsModel>();

            foreach (var forumPost in list)
            {
                string posted;
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                {
                    var languageCode = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;
                    var postedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
                    posted = string.Format(await _localizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postedAgo);
                }
                else
                {
                    posted = (await _dateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
                }

                var topic = await _forumService.GetTopicByIdAsync(forumPost.TopicId);

                latestPosts.Add(new PostsModel
                {
                    ForumTopicId = topic.Id,
                    ForumTopicTitle = topic.Subject,
                    ForumTopicSlug = await _forumService.GetTopicSeNameAsync(topic),
                    ForumPostText = _forumService.FormatPostText(forumPost),
                    Posted = posted
                });
            }

            var pagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "UserProfilePaged",
                UseRouteLinks = true,
                RouteValues = new RouteValues { PageNumber = page, Id = user.Id }
            };

            var model = new ProfilePostsModel
            {
                PagerModel = pagerModel,
                Posts = latestPosts,
            };

            return model;
        }

        #endregion
    }
}