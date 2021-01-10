using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Configuration;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Классы, реализующие этот интерфейс могут быть обслужены как портал для различных сервисов, входящих в движок TVProgViewer. 
    /// Редактирование функциональности, моудлей и реализацией доступа самого TVProgViewer через этот интерфейс.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Добавить и сконфигурировать сервисы
        /// </summary>
        /// <param name="services">Коллекция дескрипторов сервисов</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="TvProgConfig">Конфигурация параметров</param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration, TvProgConfig tvProgConfig);

        /// <summary>
        /// Конфигурация HTTP запрос 
        /// </summary>
        /// <param name="application">Конструктор для настройки конвейера запросов приложения</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

        /// <summary>
        /// Разрешение зависимостей
        /// </summary>
        /// <typeparam name="T">Тип разрешаемого сервиса</typeparam>
        /// <returns>Разрешенные сервисы</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Разрешение сервисов
        /// </summary>
        /// <param name="type">Тип разрешённых сервисов</param>
        /// <returns>Разрешённые сервисы</returns>
        object Resolve(Type type);

        /// <summary>
        /// Разрешение зависимостей
        /// </summary>
        /// <typeparam name="T">Тип разрешённых сервисов</typeparam>
        /// <returns>Коллекция разрешённых сервисов</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Разрешение нерегистрированных сервисов
        /// </summary>
        /// <param name="type">Тип сервиса</param>
        /// <returns>Разрешение нерегистрированных сервисов</returns>
        object ResolveUnregistered(Type type);

        /// <summary>
        /// Регистрация зависимостей
        /// </summary>
        /// <param name="containerBuilder">Контейнер-строитель</param>
        /// <param name="nopConfig">Параметры конфигурации TvProgViewer</param>
        void RegisterDependencies(ContainerBuilder containerBuilder, TvProgConfig nopConfig);

    }
}
