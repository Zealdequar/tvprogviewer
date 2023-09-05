
namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Хранилище пиктограмм
    /// </summary>
    public partial class MediaPic: BaseEntity
    {
        /// <summary>
        /// Наименование файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Тип содержимого
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Кодировка содержимого
        /// </summary>
        public string ContentCoding { get; set; }

        /// <summary>
        /// Длина файла
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Длина файла 25x25 
        /// </summary>
        public int? Length25 { get; set; }

        /// <summary>
        /// Индицирует, системная ли пиктограмма
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Путь к оригинальной картинке
        /// </summary>
        public string PathOrig { get; set; }

        /// <summary>
        /// Путь к картинке 25x25
        /// </summary>
        public string Path25 { get; set; }
    }
}
