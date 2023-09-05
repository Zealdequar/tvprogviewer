using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
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
