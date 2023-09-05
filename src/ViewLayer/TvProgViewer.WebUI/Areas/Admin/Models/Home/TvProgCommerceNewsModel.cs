using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a tvProgViewer news model
    /// </summary>
    public partial record TvProgViewerNewsModel : BaseTvProgModel
    {
        #region Ctor

        public TvProgViewerNewsModel()
        {
            Items = new List<TvProgViewerNewsDetailsModel>();
        }

        #endregion

        #region Properties

        public List<TvProgViewerNewsDetailsModel> Items { get; set; }

        public bool HasNewItems { get; set; }

        public bool HideAdvertisements { get; set; }

        #endregion
    }
}