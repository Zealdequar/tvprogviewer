using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer product search model
    /// </summary>
    public partial record ManufacturerProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        #endregion
    }
}