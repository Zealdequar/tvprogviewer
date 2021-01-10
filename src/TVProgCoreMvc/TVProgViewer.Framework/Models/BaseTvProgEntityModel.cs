
namespace TVProgViewer.Web.Framework.Models
{
    /// <summary>
    /// Represents base TvProg entity model
    /// </summary>
    public partial record BaseTvProgEntityModel : BaseTvProgModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}