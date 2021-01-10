namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Веб-ресурсы источники телепрограммы
    /// </summary>
    public partial class WebResources: BaseEntity
    {
        /// <summary>
        /// Идентификатор типа телепрограмы
        /// </summary>
        public int TypeProgId { get; set; }

        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Наименование ресурса
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// URL ресурса
        /// </summary>
        public string ResourceUrl { get; set; }
    }
}
