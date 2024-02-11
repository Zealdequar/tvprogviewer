using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record CompareTvChannelsModel : BaseTvProgEntityModel
    {
        public CompareTvChannelsModel()
        {
            TvChannels = new List<TvChannelOverviewModel>();
        }
        public IList<TvChannelOverviewModel> TvChannels { get; set; }

        public bool IncludeShortDescriptionInCompareTvChannels { get; set; }
        public bool IncludeFullDescriptionInCompareTvChannels { get; set; }
    }
}