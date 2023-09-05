using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.DiscountRules.UserRoles.Models
{
    public class RequirementModel
    {
        public RequirementModel()
        {
            AvailableUserRoles = new List<SelectListItem>();
        }

        [TvProgResourceDisplayName("Plugins.DiscountRules.UserRoles.Fields.UserRole")]
        public int UserRoleId { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }
    }
}