using Microsoft.AspNetCore.Mvc;

namespace TvProgViewer.Web.Framework.Mvc
{
    /// <summary>
    /// Null JSON result
    /// </summary>
    public partial class NullJsonResult : JsonResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NullJsonResult() : base(null)
        {
        }
    }
}
