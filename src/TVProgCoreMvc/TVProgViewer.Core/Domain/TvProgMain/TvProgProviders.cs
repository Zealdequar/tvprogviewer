namespace TVProgViewer.Core.Domain.TvProgMain
{
    public partial class TvProgProviders: BaseEntity
    {
        /// <summary>
        /// Наименование провайдера программы телепередач
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Веб-сайт провайдера
        /// </summary>
        public string ProviderWebSite { get; set; }

        /// <summary>
        /// Контактные данные
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// Эл. почтовый адрес контактного лица
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// RSS канал
        /// </summary>
        public string Rss { get; set; }
    }
}
