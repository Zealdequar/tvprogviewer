using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Infrastructure.DependencyManagement;
using TVProgViewer.Core.Infrastructure.Mapper;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Представляет TVProgViewer движок
    /// </summary>
    public class TvProgEngine : IEngine
    {

        #region Поля

        private ITypeFinder _typeFinder;

        #endregion

        #region Утилиты

        /// <summary>
        /// Получает IServiceProvider
        /// </summary>
        /// <returns>IServiceProvider</returns>
        protected IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider == null)
                return null;
            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            var context = accessor?.HttpContext;
            return context?.RequestServices ?? ServiceProvider;
        }

        /// <summary>
        /// Запускает стартовые задачи
        /// </summary>
        /// <param name="typeFinder">Тип поисковик</param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            //ищет стартовые задачи, обеспечиваемые другими сборками
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            //создание и сортировка экземпляров стартовых задач
            //мы запускаем эти интерфейсы даже для неустановленных плагинов
            //в противном случае, DbContext инициализаторы не будут работать и установка плагина не сработает 
            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            // Выполнение задач:
            foreach (var task in instances)
                task.ExecuteAsync().Wait();
        }

        /// <summary>
        /// Регистрация зависимостей
        /// </summary>
        /// <param name="containerBuilder">Контейнер-строитель</param>
        /// <param name="TvProgConfig">TVProgViewer параметры конфигурации</param>
        public virtual void RegisterDependencies(ContainerBuilder containerBuilder, AppSettings appSettings)
        {
            // Регистрация движка:
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            // Регистрация поисковика типов
            containerBuilder.RegisterInstance(_typeFinder).As<ITypeFinder>().SingleInstance();

            // Поиск зависимости регистраций, обеспечиваемых другими сборками
            var dependencyRegistrars = _typeFinder.FindClassesOfType<IDependencyRegistrar>();

            // Создание и сортировка экземпляров зависимости регистраций
            var instances = dependencyRegistrars
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            // Регистрация всех обеспечиваемых зависимостей
            foreach (var dependencyRegistrar in instances)
                dependencyRegistrar.Register(containerBuilder, _typeFinder, appSettings);
        }

        /// <summary>
        /// Регистрация и конфигурирование Автомаппера
        /// </summary>
        /// <param name="services">Коллекция указателей на сервисы</param>
        /// <param name="typeFinder">Тип поисковик</param>
        protected virtual void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
        {
            // Ищем конфигурацию маппера, обеспечиваемую другими сборками:
            var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

            // Создание и сортировка экземпляров конфигураций маппера:
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            // Создание конфигурации автомаппера:
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            // Регистрация:
            AutoMapperConfiguration.Init(config);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Проверка, что сборка уже загружена
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            // Получение сборки от TypeFinder
            var tf = Resolve<ITypeFinder>();
            if (tf == null)
                return null;

            assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление и конфигурирование сервисов
        /// </summary>
        /// <param name="services">Коллекция указателей на сервисы</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="TvProgConfig">Параметры конфигурации TVProgViewer</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Поиск стартовых конфигураций, обеспечиваемых другиими сборками:
            _typeFinder = new WebAppTypeFinder();
            var startupConfigurations = _typeFinder.FindClassesOfType<ITvProgStartup>();

            // Создание и сортировка экземпляров стартовых конфигураций
            var instances = startupConfigurations
                .Select(startup => (ITvProgStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            // Конфигурирование сервисов
            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);

            // Регистрация конфигураций маппера
            AddAutoMapper(services, _typeFinder);

            // Запуск стартовых задач
            RunStartupTasks(_typeFinder);

            // Здесь разрешаются сборки. В противном случае, плагины могут сгенерить исключение во время рендеринга представлений
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        /// <summary>
        /// Конфигурирование конвейера HTTP-запросов
        /// </summary>
        /// <param name="application">Строитель для конфигурирования конвейера запросов приложения</param>
        public void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            ServiceProvider = application.ApplicationServices;

            // Поиск стартовых конфигураций, обеспечиваемых другими сборками:
            var typeFinder = Resolve<ITypeFinder>();
            var startupConfigurations = typeFinder.FindClassesOfType<ITvProgStartup>();

            // Создание и сортировка экземпляров стартовых конфигураций:
            var instances = startupConfigurations
                .Select(startup => (ITvProgStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            // Конфигурирование конвейера запросов
            foreach (var instance in instances)
                instance.Configure(application);
        }

        /// <summary>
        /// Разрешение зависимости
        /// </summary>
        /// <typeparam name="T">Тип разрешаемого сервиса</typeparam>
        /// <returns>Разрешенный сервис</returns>
        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Разрешение зависимости
        /// </summary>
        /// <param name="type">Тип разрешенного сервиса</param>
        /// <returns>Разрешенный сервис</returns>
        public object Resolve(Type type)
        {
            var sp = GetServiceProvider();

            if (sp == null)
                return null;

            return sp.GetService(type);
        }

        /// <summary>
        /// Разрешение зависимостей
        /// </summary>
        /// <typeparam name="T">Тип разрешённыз сервисов</typeparam>
        /// <returns>Коллекция разрешённых сервисов</returns>
        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        /// <summary>
        /// Разрешение незарегистрированных сервисов
        /// </summary>
        /// <param name="type">Тип сервиса</param>
        /// <returns>Разрешенный сервис</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    // Попытка разрешить параметры конструктора:
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new TvProgException("Неизвестная зависимость");
                        return service;
                    });

                    // Всё ок, так что создадим инстанс
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new TvProgException("Не был найден конструктор, который удовлетворял всем зависимостям.", innerException);
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Провайдер сервисов
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; protected set; }

        #endregion
    }
}