using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record TvTypeProgSelectorModel: BaseTvProgModel
    {
        public TvTypeProgSelectorModel()
        {
            AvaliableTypes = new List<TvTypeProgModel>();
        }

        public IList<TvTypeProgModel> AvaliableTypes { get; set; }

        public int CurrentTypeProgId { get; set; }
    }
}
