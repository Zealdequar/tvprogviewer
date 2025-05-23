﻿namespace TvProgViewer.TvProgUpdaterV3.Validators
{
    /// <summary>
    /// Represents default values related to validation
    /// </summary>
    public static partial class TvProgValidationDefaults
    {
        /// <summary>
        /// Gets the name of a rule set used to validate model
        /// </summary>
        public static string ValidationRuleSet => "Validate";

        /// <summary>
        /// Gets the name of a locale used in not-null validation
        /// </summary>
        public static string NotNullValidationLocaleName => "Admin.Common.Validation.NotEmpty";
    }
}