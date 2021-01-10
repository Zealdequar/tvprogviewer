using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user shopping cart search model
    /// </summary>
    public partial record UserShoppingCartSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserShoppingCartSearchModel()
        {
            AvailableShoppingCartTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
        public int ShoppingCartTypeId { get; set; }

        public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }

        #endregion
    }
}