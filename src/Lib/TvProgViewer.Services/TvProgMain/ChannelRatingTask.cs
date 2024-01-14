using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvProgViewer.Services.ScheduleTasks;

namespace TvProgViewer.Services.TvProgMain
{
    public partial class ChannelRatingTask : IScheduleTask
    {
        #region Поля

        private readonly IChannelService _channelService;

        #endregion

        #region Конструктор

        public ChannelRatingTask(IChannelService channelService)
        {
            _channelService = channelService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Выполнение задачи
        /// </summary>
        public async Task ExecuteAsync()
        {
            await _channelService.RecalculateChannelUserRatingAsync();
            await _channelService.ReorderChannelAsync();
        }

        #endregion
    }
}
