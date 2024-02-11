using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record SearchBoxModel : BaseTvProgModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowTvChannelImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
        public bool ShowSearchBox { get; set; }
    }
}