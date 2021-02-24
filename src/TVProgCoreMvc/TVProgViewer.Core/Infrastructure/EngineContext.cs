using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class EngineContext
    {
        #region Методы

        /// <summary>
        /// Создание статического инстанса TVProgViewer движка
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            //Создание движка
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new TvProgEngine());
        }

        /// <summary>
        /// Установка статическому движку инстанса поддерживаемого движка. Используйте этот метод, чтобы создать свою собственную реализацию
        /// </summary>
        /// <param name="engine">Движок для использования</param>
        /// <remarks>Используй этот метод, только если Вы знаете, что Вы делаете</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Получение одиночки TVProgViewer движка для использования доступа к сервисам
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
