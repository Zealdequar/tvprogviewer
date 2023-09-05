using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product model to associate to the product attribute value
    /// </summary>
    public partial record AssociateProductToAttributeValueModel : BaseTvProgModel
    {
        #region Properties
        
        public int AssociatedToProductId { get; set; }

        #endregion
    }
}