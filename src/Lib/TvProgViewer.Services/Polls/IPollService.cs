using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Polls;

namespace TvProgViewer.Services.Polls
{
    /// <summary>
    /// Poll service interface
    /// </summary>
    public partial interface IPollService
    {
        /// <summary>
        /// Gets a poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the poll
        /// </returns>
        Task<Poll> GetPollByIdAsync(int pollId);

        /// <summary>
        /// Gets polls
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="languageId">Language identifier; pass 0 to load all records</param>
        /// <param name="showHidden">Whether to show hidden records (not published, not started and expired)</param>
        /// <param name="loadShownOnHomepageOnly">Retrieve only shown on home page polls</param>
        /// <param name="systemKeyword">The poll system keyword; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the polls
        /// </returns>
        Task<IPagedList<Poll>> GetPollsAsync(int storeId, int languageId = 0, bool showHidden = false,
            bool loadShownOnHomepageOnly = false, string systemKeyword = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes a poll
        /// </summary>
        /// <param name="poll">The poll</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePollAsync(Poll poll);

        /// <summary>
        /// Inserts a poll
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPollAsync(Poll poll);

        /// <summary>
        /// Updates the poll
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdatePollAsync(Poll poll);

        /// <summary>
        /// Gets a poll answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the poll answer
        /// </returns>
        Task<PollAnswer> GetPollAnswerByIdAsync(int pollAnswerId);

        /// <summary>
        /// Gets a poll answers by parent poll
        /// </summary>
        /// <param name="pollId">The poll identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IPagedList<PollAnswer>> GetPollAnswerByPollAsync(int pollId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePollAnswerAsync(PollAnswer pollAnswer);

        /// <summary>
        /// Inserts a poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPollAnswerAsync(PollAnswer pollAnswer);

        /// <summary>
        /// Updates the poll answer
        /// </summary>
        /// <param name="pollAnswer">Poll answer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdatePollAnswerAsync(PollAnswer pollAnswer);

        /// <summary>
        /// Gets a value indicating whether user already voted for this poll
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> AlreadyVotedAsync(int pollId, int userId);

        /// <summary>
        /// Inserts a poll voting record
        /// </summary>
        /// <param name="pollVotingRecord">Voting record</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPollVotingRecordAsync(PollVotingRecord pollVotingRecord);

        /// <summary>
        /// Gets a poll voting records by parent answer
        /// </summary>
        /// <param name="pollAnswerId">Poll answer identifier</param>
        /// <returns>Poll answer</returns>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IPagedList<PollVotingRecord>> GetPollVotingRecordsByPollAnswerAsync(int pollAnswerId, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}