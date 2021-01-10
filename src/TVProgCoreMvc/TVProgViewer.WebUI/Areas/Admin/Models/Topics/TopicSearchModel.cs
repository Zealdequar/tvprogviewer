using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Topics
{
    /// <summary>
    /// Represents a topic search model
    /// </summary>
    public partial record TopicSearchModel : BaseSearchModel
    {
        #region Ctor

        public TopicSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.List.SearchKeywords")]
        public string SearchKeywords { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}