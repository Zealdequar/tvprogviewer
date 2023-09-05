
namespace TvProgViewer.TvProgUpdaterV3.Models
{
    /// <summary>
    /// Delete confirmation model
    /// </summary>
    public partial record DeleteConfirmationModel : BaseTvProgModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Controller name
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Action name
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Window ID
        /// </summary>
        public string WindowId { get; set; }
    }
}