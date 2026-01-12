using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Представляет операции пользователя из GreenData
    /// </summary>
    public partial class UserGreenDataOperations: BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает наименование операции
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или устанавливает признак активности операции
        /// </summary>
        public bool Active { get; set; }
    }
}
