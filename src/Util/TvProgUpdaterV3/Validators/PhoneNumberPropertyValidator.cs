using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.TvProgUpdaterV3.Validators
{
    /// <summary>
    /// Phohe number validator
    /// </summary>
    public partial class PhoneNumberPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly UserSettings _userSettings;

        public override string Name => "PhoneNumberPropertyValidator";

        /// <summary>
        /// Ctor
        /// </summary>
        public PhoneNumberPropertyValidator(UserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            return IsValid(value as string, _userSettings);
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="phoneNumber">SmartPhone number</param>
        /// <param name="userSettings">User settings</param>
        /// <returns>Result</returns>
        public static bool IsValid(string phoneNumber, UserSettings userSettings)
        {
            if (!userSettings.PhoneNumberValidationEnabled || string.IsNullOrEmpty(userSettings.PhoneNumberValidationRule))
                return true;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return !userSettings.SmartPhoneRequired;
            }

            return userSettings.PhoneNumberValidationUseRegex
                ? Regex.IsMatch(phoneNumber, userSettings.PhoneNumberValidationRule, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                : phoneNumber.All(l => userSettings.PhoneNumberValidationRule.Contains(l));
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "SmartPhone number is not valid";
    }
}
