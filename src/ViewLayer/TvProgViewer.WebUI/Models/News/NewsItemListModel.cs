using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.News
{
    public partial record NewsItemListModel : BaseTvProgModel
    {
        public NewsItemListModel()
        {
            PagingFilteringContext = new NewsPagingFilteringModel();
            NewsItems = new List<NewsItemModel>();
        }

        public int WorkingLanguageId { get; set; }
        public NewsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<NewsItemModel> NewsItems { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
    }
}