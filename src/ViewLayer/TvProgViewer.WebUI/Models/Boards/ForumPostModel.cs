﻿using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Boards
{
    public partial record ForumPostModel : BaseTvProgModel
    {
        public int Id { get; set; }
        public int ForumTopicId { get; set; }
        public string ForumTopicSeName { get; set; }

        public string FormattedText { get; set; }

        public bool IsCurrentUserAllowedToEditPost { get; set; }
        public bool IsCurrentUserAllowedToDeletePost { get; set; }

        public int UserId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string UserAvatarUrl { get; set; }
        public string UserName { get; set; }
        public bool IsUserForumModerator { get; set; }

        public string PostCreatedOnStr { get; set; }

        public bool ShowUsersPostCount { get; set; }
        public int ForumPostCount { get; set; }

        public bool ShowUsersJoinDate { get; set; }
        public DateTime UserJoinDate { get; set; }

        public bool ShowUsersLocation { get; set; }
        public string UserLocation { get; set; }

        public bool AllowPrivateMessages { get; set; }

        public bool SignaturesEnabled { get; set; }
        public string FormattedSignature { get; set; }

        public int CurrentTopicPage { get; set; }

        public bool AllowPostVoting { get; set; }
        public int VoteCount { get; set; }
        public bool? VoteIsUp { get; set; }
    }
}