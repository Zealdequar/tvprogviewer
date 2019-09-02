using System.Configuration;
using System.Data.Common;
using AutoMapper;

namespace TVProgViewer.DataAccess.Adapters
{
    /// <summary>
    /// Базовый класс адаптера
    /// </summary>
    public class AdapterBase
    {
        /// <summary>
        /// Контекст данных EF
        /// </summary>
        protected TVProgBaseEntities dataContext;
        
        /// <summary>
        /// Маппер
        /// </summary>
        protected IMapper mapper;

        /// <summary>
        /// Улучшения доступа к данным
        /// </summary>
        protected DataAccess da = new DataAccess();

        /// <summary>
        /// Конструктор
        /// </summary>
        public AdapterBase()
        {
           dataContext  = new TVProgBaseEntities();
           dataContext.Database.CommandTimeout = 180;
           var config = new MapperConfiguration(cfg => { cfg.AllowNullCollections = true; });
           mapper = config.CreateMapper();
        }

        /// <summary>
        /// Получить главное соединение
        /// </summary>
        /// <returns>Подключение</returns>
        public static DbConnection GetTvProgMainConnection()
        {
           return GetConnection("tvProgBase.Main"); 
        }

        /// <summary>
        /// Получить безопасное соединение
        /// </summary>
        /// <returns>Подключение</returns>
        public static DbConnection GetTvProgSecureConnection()
        {
           return GetConnection("tvProgBase.Secure");
        }

        /// <summary>
        /// Получение соединения
        /// </summary>
        /// <param name="name">Название подключения в файле конфигурации</param>
        /// <returns>Подключение</returns>
        private static DbConnection GetConnection(string name)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);
            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = settings.ConnectionString;
            return conn;
        }
    }
}
