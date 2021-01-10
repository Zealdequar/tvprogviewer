using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record TaxTypeSelectorModel : BaseTvProgModel
    {
        public TaxDisplayType CurrentTaxType { get; set; }
    }
}