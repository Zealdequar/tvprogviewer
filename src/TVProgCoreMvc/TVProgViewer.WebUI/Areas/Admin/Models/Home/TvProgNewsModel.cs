using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Home
{
    /// <summary>
    /// Represents a nopCommerce news model
    /// </summary>
    public partial record TvProgNewsModel : BaseTvProgModel
    {
        #region Ctor

        public TvProgNewsModel()
        {
            Items = new List<TvProgNewsDetailsModel>();
        }

        #endregion

        #region Properties

        public List<TvProgNewsDetailsModel> Items { get; set; }

        public bool HasNewItems { get; set; }

        public bool HideAdvertisements { get; set; }

        #endregion
    }
}