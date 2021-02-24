using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Services.Tasks;
using TVProgViewer.Web.Framework.Infrastructure.Extensions;

namespace TVProgViewer.WebUI
{
    /// <summary>
    /// Представляет запускной класс приложения
    /// </summary>
    public class Startup
    {
        #region Поля

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private AppSettings _appSettings;
        private IEngine _engine;

        #endregion

        #region Конструктор

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// Добавление сервисов к приложению и конфигурирование провайдера сервисов
        /// </summary>
        /// <param name="services">Коллекция указателей на сервисы</param>
        public void ConfigureServices(IServiceCollection services)
        {
            (_engine, _appSettings) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
        }

        /// <summary>
        /// Конфигурирование DI-контейнера 
        /// </summary>
        /// <param name="builder">Container builder</param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            _engine.RegisterDependencies(builder, _appSettings);
        }

        /// <summary>
        /// Конфигурирование приложения конвейера HTTP-запросов
        /// </summary>
        /// <param name="applicaton">Строитель для конфигурирования приложения конвейера запросов</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
            application.StartEngine();
        }
    }
}