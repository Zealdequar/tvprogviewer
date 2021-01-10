using Newtonsoft.Json;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Core.Redis;
using StackExchange.Redis;

namespace TVProgViewer.Services.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public partial class RedisPluginsInfo : PluginsInfo
    {
        #region Fields

        private readonly IDatabase _db;

        #endregion

        #region Ctor

        public RedisPluginsInfo(ITvProgFileProvider fileProvider, IRedisConnectionWrapper connectionWrapper, TvProgConfig config)
            : base(fileProvider)
        {
            _db = connectionWrapper.GetDatabase(config.RedisDatabaseId ?? (int)RedisDatabaseNumber.Plugin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save plugins info to the redis
        /// </summary>
        public override void Save()
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            _db.StringSet(nameof(RedisPluginsInfo), text);
        }

        /// <summary>
        /// Get plugins info
        /// </summary>
        /// <returns>True if data are loaded, otherwise False</returns>
        public override bool LoadPluginInfo()
        {
            //try to get plugin info from the JSON file
            var serializedItem = _db.StringGet(nameof(RedisPluginsInfo));

            var loaded = false;

            if (serializedItem.HasValue) 
                loaded = DeserializePluginInfo(serializedItem);

            if (loaded)
                return true;

            if (base.LoadPluginInfo())
            {
                Save();
                loaded = true;
            }

            //delete the plugins info file
            var filePath = _fileProvider.MapPath(TvProgPluginDefaults.PluginsInfoFilePath);
            _fileProvider.DeleteFile(filePath);
            
            return loaded;
        }

        #endregion
    }
}
