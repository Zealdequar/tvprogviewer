using System;
using System.Collections.Generic;
using System.Text;

namespace TVProgViewer.Core.Infrastructure.Mapper
{
    /// <summary>
    /// Интерфейс профиля регистрации маппера
    /// </summary>
    public interface IOrderedMapperProfile
    {
        /// <summary>
        /// Получение порядка конфигурационных реализаций
        /// </summary>
        int Order { get; }
    }
}
