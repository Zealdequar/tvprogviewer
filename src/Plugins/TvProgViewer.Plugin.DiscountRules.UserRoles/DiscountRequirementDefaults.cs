
namespace TvProgViewer.Plugin.DiscountRules.UserRoles
{
    /// <summary>
    /// Represents defaults for the discount requirement rule
    /// </summary>
    public static class DiscountRequirementDefaults
    {
        /// <summary>
        /// The system name of the discount requirement rule
        /// </summary>
        public static string SystemName => "DiscountRequirement.MustBeAssignedToUserRole";

        /// <summary>
        /// The key of the settings to save restricted user roles
        /// </summary>
        public static string SettingsKey => "DiscountRequirement.MustBeAssignedToUserRole-{0}";

        /// <summary>
        /// The HTML field prefix for discount requirements
        /// </summary>
        public static string HtmlFieldPrefix => "DiscountRulesUserRoles{0}";
    }
}
