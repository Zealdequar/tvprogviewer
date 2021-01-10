using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Polls;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Factories;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class PollController : BasePublicController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IPollModelFactory _pollModelFactory;
        private readonly IPollService _pollService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PollController(IUserService userService, 
            ILocalizationService localizationService, 
            IPollModelFactory pollModelFactory,
            IPollService pollService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _userService = userService;
            _localizationService = localizationService;
            _pollModelFactory = pollModelFactory;
            _pollService = pollService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new { error = "No poll answer found with the specified id" });

            var poll = _pollService.GetPollById(pollAnswer.PollId);

            if (!poll.Published || !_storeMappingService.Authorize(poll))
                return Json(new { error = "Poll is not available" });

            if (_userService.IsGuest(_workContext.CurrentUser) && !poll.AllowGuestsToVote)
                return Json(new { error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote") });

            var alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentUser.Id);
            if (!alreadyVoted)
            {
                //vote
                _pollService.InsertPollVotingRecord(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    UserId = _workContext.CurrentUser.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //update totals
                pollAnswer.NumberOfVotes = _pollService.GetPollVotingRecordsByPollAnswer(pollAnswer.Id).Count;
                _pollService.UpdatePoll(poll);
            }

            return Json(new
            {
                html = RenderPartialViewToString("_Poll", _pollModelFactory.PreparePollModel(poll, true)),
            });
        }

        #endregion
    }
}