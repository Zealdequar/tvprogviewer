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

        private readonly IRepository<TvChannelTemplate> _tvChannelTemplateRepository;

        #endregion

        #region Ctor

        public TvChannelTemplateService(IRepository<TvChannelTemplate> tvChannelTemplateRepository)
        {
            _tvChannelTemplateRepository = tvChannelTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate)
        {
            await _tvChannelTemplateRepository.DeleteAsync(tvChannelTemplate);
        }

        /// <summary>
        /// Gets all tvChannel templates
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel templates
        /// </returns>
        public virtual async Task<IList<TvChannelTemplate>> GetAllTvChannelTemplatesAsync()
        {
            var templates = await _tvChannelTemplateRepository.GetAllAsync(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplateId">TvChannel template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel template
        /// </returns>
        public virtual async Task<TvChannelTemplate> GetTvChannelTemplateByIdAsync(int tvChannelTemplateId)
        {
            return await _tvChannelTemplateRepository.GetByIdAsync(tvChannelTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate)
        {
            await _tvChannelTemplateRepository.InsertAsync(tvChannelTemplate);
        }

        /// <summary>
        /// Updates the tvChannel template
        /// </summary>
        /// <param name="tvChannelTemplate">TvChannel template</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelTemplateAsync(TvChannelTemplate tvChannelTemplate)
        {
            await _tvChannelTemplateRepository.UpdateAsync(tvChannelTemplate);
        }

        #endregion
    }
}