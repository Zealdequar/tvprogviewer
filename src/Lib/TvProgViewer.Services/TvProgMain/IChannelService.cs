using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Интерфейс для работы с телеканалами
    /// </summary>
    public partial interface IChannelService
    {
        /// <summary>
        /// Получение системных телеканалов
        /// </summary>
        /// <param name="tvProgProviderId">Идентификатор провайдера телеканалов</param>
        public Task<KeyValuePair<int, List<SystemChannel>>> GetSystemChannelsAsync(int tvProgProvider, string filtData, string sidx, string sord, int page, int rows);


        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="tvProgProvder">Идентификатор провайдера телеканалов</param>
        /// <param name="jsonChannels">Данные из localStorage</param>
        public Task<List<UserChannel>> GetUserChannelsByLocalStorageAsync(int tvProgProvder, string jsonChannels);

        /// <summary>
        /// Перерасчёт пользовательского рейтинга телеканалов
        /// </summary>
        public Task RecalculateChannelUserRatingAsync();

        /// <summary>
        /// Пересортировка телеканалов в соответствии с пользовательским рейтингом
        /// </summary>
        public Task ReorderChannelAsync();
    }
}
