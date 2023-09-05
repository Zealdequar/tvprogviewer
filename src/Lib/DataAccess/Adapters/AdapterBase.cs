using AutoMapper;
using TvProgViewer.DataAccess.Models;

namespace TvProgViewer.DataAccess.Adapters
{
    /// <summary>
    /// Базовый класс адаптера
    /// </summary>
    public class AdapterBase
    {
        /// <summary>
        /// Контекст данных EF
        /// </summary>
        protected TVProgBaseProdContext dataContext;
        
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
           dataContext  = new TVProgBaseProdContext();
           
           var config = new MapperConfiguration(cfg => { cfg.AllowNullCollections = true; /*cfg.CreateMissingTypeMaps = true; */
           });
           mapper = config.CreateMapper();
            
        }

        
    }
}