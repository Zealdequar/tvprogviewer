using Microsoft.AspNetCore.Mvc;

namespace TvProgViewer.TvProgUpdaterV3.Mvc
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
