using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel template service
    /// </summary>
    public partial class TvChannelTemplateService : ITvChannelTemplateService
    {
        #region Fields

        private readonly IRepository<TvChannelTemplate> _tvchannelTemplateRepository;

        #endregion

        #region Ctor

        public TvChannelTemplateService(IRepository<TvChannelTemplate> tvchannelTemplateRepository)
        {
            _tvchannelTemplateRepository = tvchannelTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate)
        {
            await _tvchannelTemplateRepository.DeleteAsync(tvchannelTemplate);
        }

        /// <summary>
        /// Gets all tvchannel templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel templates
        /// </returns>
        public virtual async Task<IList<TvChannelTemplate>> GetAllTvChannelTemplatesAsync()
        {
            var templates = await _tvchannelTemplateRepository.GetAllAsync(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplateId">TvChannel template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel template
        /// </returns>
        public virtual async Task<TvChannelTemplate> GetTvChannelTemplateByIdAsync(int tvchannelTemplateId)
        {
            return await _tvchannelTemplateRepository.GetByIdAsync(tvchannelTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate)
        {
            await _tvchannelTemplateRepository.InsertAsync(tvchannelTemplate);
        }

        /// <summary>
        /// Updates the tvchannel template
        /// </summary>
        /// <param name="tvchannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelTemplateAsync(TvChannelTemplate tvchannelTemplate)
        {
            await _tvchannelTemplateRepository.UpdateAsync(tvchannelTemplate);
        }

        #endregion
    }
}