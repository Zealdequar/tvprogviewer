using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Data;
using TvProgViewer.Plugin.Payments.CyberSource.Domain;

namespace TvProgViewer.Plugin.Payments.CyberSource.Services
{
    /// <summary>
    /// Represents CyberSource user token service
    /// </summary>
    public class UserTokenService
    {
        #region Fields

        private readonly IRepository<CyberSourceUserToken> _userTokenRepository;

        #endregion

        #region Ctor

        public UserTokenService(IRepository<CyberSourceUserToken> userTokenRepository)
        {
            _userTokenRepository = userTokenRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all CyberSource user token records
        /// </summary>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="instrumentIdentifier">Instrument identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the CyberSource user token record
        /// </returns>
        public async Task<IList<CyberSourceUserToken>> GetAllTokensAsync(int userId = 0, string instrumentIdentifier = null)
        {
            return await _userTokenRepository.GetAllAsync(query =>
            {
                if (userId > 0)
                    query = query.Where(token => token.UserId == userId);

                if (!string.IsNullOrEmpty(instrumentIdentifier))
                    query = query.Where(token => token.InstrumentIdentifier == instrumentIdentifier);

                return query;
            }, null);
        }

        /// <summary>
        /// Get a CyberSource user token record by identifier
        /// </summary>
        /// <param name="id">Record identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the CyberSource user token record
        /// </returns>
        public async Task<CyberSourceUserToken> GetByIdAsync(int id)
        {
            return await _userTokenRepository.GetByIdAsync(id, null);
        }

        /// <summary>
        /// Insert the CyberSource user token record
        /// </summary>
        /// <param name="cyberSourceUserToken">CyberSource user token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertAsync(CyberSourceUserToken cyberSourceUserToken)
        {
            await _userTokenRepository.InsertAsync(cyberSourceUserToken, false);
        }

        /// <summary>
        /// Update the CyberSource user token record
        /// </summary>
        /// <param name="cyberSourceUserToken">CyberSource user token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateAsync(CyberSourceUserToken cyberSourceUserToken)
        {
            await _userTokenRepository.UpdateAsync(cyberSourceUserToken, false);
        }

        /// <summary>
        /// Delete the CyberSource user token record
        /// </summary>
        /// <param name="cyberSourceUserToken">CyberSource user token record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteAsync(CyberSourceUserToken cyberSourceUserToken)
        {
            await _userTokenRepository.DeleteAsync(cyberSourceUserToken, false);
        }

        #endregion
    }
}