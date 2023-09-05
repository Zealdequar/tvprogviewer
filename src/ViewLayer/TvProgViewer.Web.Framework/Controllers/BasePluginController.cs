using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for plugins
    /// </summary>
    [NotNullValidationMessage]
    public abstract partial class BasePluginController : BaseController
    {
    }
}
