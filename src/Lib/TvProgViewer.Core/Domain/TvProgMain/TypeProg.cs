namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Тип телепрограммы
    /// </summary>
    public partial class TypeProg: BaseEntity
    {
        /// <summary>
        /// Провайдер телепрограммы
        /// </summary>
        public int TvProgProviderId { get; set; }
        
        /// <summary>
        /// Тип телепрограммы
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Формат файла
        /// </summary>
        public string FileFormat { get; set; }
    }
}
