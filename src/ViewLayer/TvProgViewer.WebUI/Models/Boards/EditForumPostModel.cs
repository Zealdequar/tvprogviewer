﻿using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Boards
{
    public partial record EditForumPostModel : BaseTvProgModel
    {
        #region Properties

        public int Id { get; set; }

        public int ForumTopicId { get; set; }

        public bool IsEdit { get; set; }

        public string Text { get; set; }

        public EditorType ForumEditor { get; set; }

        public string ForumName { get; set; }

        public string ForumTopicSubject { get; set; }

        public string ForumTopicSeName { get; set; }

        public bool IsUserAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }

        public bool DisplayCaptcha { get; set; } 

        #endregion
    }
}