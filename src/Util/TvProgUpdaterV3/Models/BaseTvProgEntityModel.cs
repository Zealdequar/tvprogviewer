
namespace TvProgViewer.TvProgUpdaterV3.Models
{
    /// <summary>
    /// Represents base tvProgViewer entity model
    /// </summary>
    public partial record BaseTvProgEntityModel : BaseTvProgModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}