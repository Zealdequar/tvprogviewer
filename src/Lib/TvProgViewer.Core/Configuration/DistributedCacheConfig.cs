using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TvProgViewer.Core.Configuration
{
    /// <summary>
    /// Represents distributed cache configuration parameters
    /// </summary>
    public partial class DistributedCacheConfig : IConfig
    {
        /// <summary>
        /// Получение или установка типа распределённого кэша
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DistributedCacheType DistributedCacheType { get; private set; } = DistributedCacheType.RedisSynchronizedMemory;

        /// <summary>
        /// Gets or sets a value indicating whether we should use distributed cache
        /// </summary>
        public bool Enabled { get; private set; } = false;

        /// <summary>
        /// Gets or sets connection string. Used when distributed cache is enabled
        /// </summary>
        public string ConnectionString { get; private set; } = "127.0.0.1:6379,ssl=False";

        /// <summary>
        /// Gets or sets schema name. Used when distributed cache is enabled and DistributedCacheType property is set as SqlServer
        /// </summary>
        public string SchemaName { get; private set; } = "dbo";

        /// <summary>
        /// Получает или устанавливает название таблички. Используется когда включен распределённое кэширование и значение свойства DistributedCacheType установлено в SqlServer
        /// </summary>
        public string TableName { get; private set; } = "DistributedCache";

        /// <summary>
        /// Получает или устанавливает наименование экземпляра. Используется когда включен распределённое кэширование и значение свойства DistributedCacheType установлено как Redis или RedisSynchronizedMemory.
        /// Полезно, когда требуется разделить один сервер Redis для использования с несколькими приложениями, например, указав в имени экземпляра значения "разработка" и "пром".
        /// </summary>
        public string InstanceName { get; private set; } = "TvProgViewer";
    }
}