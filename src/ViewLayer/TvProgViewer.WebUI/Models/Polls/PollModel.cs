﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Polls
{
    public partial record PollModel : BaseTvProgEntityModel
    {
        public PollModel()
        {
            Answers = new List<PollAnswerModel>();
        }

        public string Name { get; set; }

        public bool AlreadyVoted { get; set; }

        public int TotalVotes { get; set; }
        
        public IList<PollAnswerModel> Answers { get; set; }
    }

    public partial record PollAnswerModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public int NumberOfVotes { get; set; }

        public double PercentOfTotalVotes { get; set; }
    }
}