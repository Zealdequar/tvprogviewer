using System;
using System.Collections.Generic;
using System.Text;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Статически компилированный "одиночка" использован для хранения объектов посредством
    /// жизненного цикла домена приложения.  Не так много паттернов одиночки 
    /// отражают слово стандартизированным путём для хранения отдельных экземпляров
    /// </summary>
    /// <typeparam name="T">Тип объекта для хранения</typeparam>
    /// <remarks>Доступ к инстансу несинхронизирован</remarks>
    public class Singleton<T> : BaseSingleton
    {
        private static T instance;

        /// <summary>
        /// Синглтон инстанс специфицированного типа Т. Только один экземпляр (за всё время) этого объекта на каждый тип Т.
        /// </summary>
        public static T Instance
        {
            get => instance;
            set
            {
                instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }
}
