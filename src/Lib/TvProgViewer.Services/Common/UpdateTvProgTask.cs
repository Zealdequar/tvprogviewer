using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.ScheduleTasks;
using TvProgViewer.Services.TvProgMain;
using TVProgViewer.Services.TvProgMain;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Представляет задачу для обновления программы телепередач
    /// </summary>
    public partial class UpdateTvProgTask : IScheduleTask
    {
        #region Fields

        private readonly StoreHttpClient _storeHttpClient;

        #endregion

        #region Ctor

        public UpdateTvProgTask(StoreHttpClient storeHttpClient)
        {
            _storeHttpClient = storeHttpClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _storeHttpClient.UpdateTvProgrammes();
        }

        #endregion
    }
}
