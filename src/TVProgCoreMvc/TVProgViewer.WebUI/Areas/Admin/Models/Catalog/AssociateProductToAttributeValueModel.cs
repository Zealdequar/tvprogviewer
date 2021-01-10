using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
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