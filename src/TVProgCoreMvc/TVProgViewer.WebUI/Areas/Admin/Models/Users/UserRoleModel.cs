using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role model
    /// </summary>
    public partial record UserRoleModel : BaseTvProgEntityModel
    {
        #region Ctor

        public UserRoleModel()
        {
            TaxDisplayTypeValues = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.FreeShipping")]
        public bool FreeShipping { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.TaxExempt")]
        public bool TaxExempt { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
        public bool Active { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.EnablePasswordLifetime")]
        public bool EnablePasswordLifetime { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.OverrideTaxDisplayType")]
        public bool OverrideTaxDisplayType { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.DefaultTaxDisplayType")]
        public int DefaultTaxDisplayTypeId { get; set; }

        public IList<SelectListItem> TaxDisplayTypeValues { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public int PurchasedWithProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public string PurchasedWithProductName { get; set; }

        #endregion
    }
}