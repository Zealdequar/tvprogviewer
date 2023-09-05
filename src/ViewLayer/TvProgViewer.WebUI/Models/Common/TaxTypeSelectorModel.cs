using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record TaxTypeSelectorModel : BaseTvProgModel
    {
        public TaxDisplayType CurrentTaxType { get; set; }
    }
}