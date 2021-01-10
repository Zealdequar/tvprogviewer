using Autofac;
using TVProgViewer.Core.Configuration;

namespace TVProgViewer.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Интерфейс регистрации зависимостей
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Регистрация сервисов или интерфейсов
        /// </summary>
        /// <param name="builder">Контейнер-строитель</param>
        /// <param name="typeFinder">Тип поисковик</param>
        /// <param name="config">Конфигурация</param>
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, TvProgConfig config);

        /// <summary>
        /// Получает порядок регистрации реализации зависимости
        /// </summary>
        int Order { get; }
    }
}
