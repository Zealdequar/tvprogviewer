using TvProgViewer.TvProgUpdaterV3.Mvc.Filters;

namespace TvProgViewer.TvProgUpdaterV3.Controllers
{
    /// <summary>
    /// Base controller for plugins
    /// </summary>
    [NotNullValidationMessage]
    public abstract partial class BasePluginController : BaseController
    {
    }
}
