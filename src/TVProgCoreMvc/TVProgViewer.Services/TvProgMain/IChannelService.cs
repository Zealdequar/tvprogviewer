using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.Data.TvProgMain.ProgObjs;

namespace TVProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Интерфейс для работы с телеканалами
    /// </summary>
    public partial interface IChannelService
    {
        /// <summary>
        /// Получение системных телеканалов
        /// </summary>
        /// <param name="tvProgProviderId">Идентификатор провайдера телканалов</param>
        public Task<KeyValuePair<int, List<SystemChannel>>> GetSystemChannelsAsync(int tvProgProvider, string sidx, string sord, int page, int rows);
    }
}
