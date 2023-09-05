﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Polls
{
    /// <summary>
    /// Represents a poll answer search model
    /// </summary>
    public partial record PollAnswerSearchModel : BaseSearchModel
    {
        #region Ctor

        public PollAnswerSearchModel()
        {
            AddPollAnswer = new PollAnswerModel();
        }

        #endregion

        #region Properties

        public int PollId { get; set; }

        public PollAnswerModel AddPollAnswer { get; set; }

        #endregion
    }
}