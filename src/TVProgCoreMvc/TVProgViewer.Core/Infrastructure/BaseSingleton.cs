using System;
using System.Collections.Generic;
using System.Text;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Обеспечивает доступ ко всем "одиночкам", хранимым в <see cref="Singleton{T}"/>.
    /// </summary>
    public class BaseSingleton
    {
        static BaseSingleton()
        {
            AllSingletons = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Словарь типов экземпляров синглтонов
        /// </summary>
        public static IDictionary<Type, object> AllSingletons { get; }
    }
}