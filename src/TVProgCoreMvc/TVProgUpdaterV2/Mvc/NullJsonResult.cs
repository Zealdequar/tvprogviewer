using Microsoft.AspNetCore.Mvc;

namespace TVProgViewer.TVProgUpdaterV2.Mvc
{
    /// <summary>
    /// Null JSON result
    /// </summary>
    public class NullJsonResult : JsonResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NullJsonResult() : base(null)
        {
        }
    }
}
