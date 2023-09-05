using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record TvProgProviderSelectorModel : BaseTvProgModel
    {
        public TvProgProviderSelectorModel()
        {
            AvaliableProviders = new List<TvProgProviderModel>();
        }

        public IList<TvProgProviderModel> AvaliableProviders { get; set; }

        public int CurrentProviderId { get; set; }
    }
}
