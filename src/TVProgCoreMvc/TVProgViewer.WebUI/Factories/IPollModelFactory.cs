using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.WebUI.Models.Polls;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the poll model factory
    /// </summary>
    public partial interface IPollModelFactory
    {
        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that user already voted for this poll</param>
        /// <returns>Poll model</returns>
        Task<PollModel> PreparePollModelAsync(Poll poll, bool setAlreadyVotedProperty);

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        Task<PollModel> PreparePollModelBySystemNameAsync(string systemKeyword);

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        Task<List<PollModel>> PrepareHomepagePollModelsAsync();
    }
}
