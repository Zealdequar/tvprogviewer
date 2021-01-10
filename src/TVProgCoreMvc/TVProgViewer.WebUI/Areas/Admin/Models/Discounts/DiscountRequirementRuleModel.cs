using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount requirement rule model
    /// </summary>
    public partial record DiscountRequirementRuleModel : BaseTvProgModel
    {
        #region Ctor

        public DiscountRequirementRuleModel()
        {
            ChildRequirements = new List<DiscountRequirementRuleModel>();
        }

        #endregion

        #region Properties

        public int DiscountRequirementId { get; set; }

        public string RuleName { get; set; }

        public string ConfigurationUrl { get; set; }

        public int InteractionTypeId { get; set; }

        public int? ParentId { get; set; }

        public SelectList AvailableInteractionTypes { get; set; }

        public bool IsGroup { get; set; }

        public bool IsLastInGroup { get; set; }

        public IList<DiscountRequirementRuleModel> ChildRequirements { get; set; }

        #endregion
    }
}