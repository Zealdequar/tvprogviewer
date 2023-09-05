using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a tvProgViewer news details model
    /// </summary>
    public partial record TvProgViewerNewsDetailsModel : BaseTvProgModel
    {
        #region Properties

        public string Title { get; set; }

        public string Url { get; set; }

        public string Summary { get; set; }

        public DateTimeOffset PublishDate { get; set; }

        #endregion
    }
}