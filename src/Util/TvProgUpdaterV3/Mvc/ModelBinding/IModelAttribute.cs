
namespace TvProgViewer.TvProgUpdaterV3.Mvc.ModelBinding
{
    /// <summary>
    /// Represents custom model attribute
    /// </summary>
    public partial interface IModelAttribute
    {
        /// <summary>
        /// Gets name of the attribute
        /// </summary>
        string Name { get; }
    }
}
