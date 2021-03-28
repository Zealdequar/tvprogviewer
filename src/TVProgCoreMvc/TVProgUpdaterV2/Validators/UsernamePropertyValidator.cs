using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation.Validators;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.TVProgUpdaterV2.Validators
{
    /// <summary>
    /// Username validator
    /// </summary>
    public class UsernamePropertyValidator : PropertyValidator
    {
        private readonly UserSettings _UserSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        public UsernamePropertyValidator(UserSettings UserSettings)
            : base("Username is not valid")
        {
            _UserSettings = UserSettings;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return IsValid(context.PropertyValue as string, _UserSettings);
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="UserSettings">User settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string username, UserSettings UserSettings)
        {
            if (!UserSettings.UsernameValidationEnabled || string.IsNullOrEmpty(UserSettings.UsernameValidationRule))
                return true;

            if (string.IsNullOrEmpty(username))
                return false;

            return UserSettings.UsernameValidationUseRegex
                ? Regex.IsMatch(username, UserSettings.UsernameValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : username.All(l => UserSettings.UsernameValidationRule.Contains(l));
        }
    }
}
