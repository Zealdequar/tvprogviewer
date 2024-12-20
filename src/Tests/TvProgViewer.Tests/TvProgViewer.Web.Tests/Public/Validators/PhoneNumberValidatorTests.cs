﻿using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Web.Framework.Validators;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators
{
    [TestFixture]
    public class PhoneNumberValidatorTests
    {
        private TestValidator _validator;
        private UserSettings _userSettings;

        [OneTimeSetUp]
        public void Setup()
        {
            _userSettings = new UserSettings
            {
                PhoneNumberValidationRule = "^[0-9]{1,14}?$",
                PhoneNumberValidationEnabled = true,
                PhoneNumberValidationUseRegex = false
            };

            _validator = new TestValidator { v => v.RuleFor(x => x.PhoneNumber).IsPhoneNumber(_userSettings) };
        }

        [Test]
        public async Task IsValidTests()
        {
            //optional value is not valid
            _userSettings.SmartPhoneRequired = true;
            var result = await _validator.ValidateAsync(new Person { PhoneNumber = null });
            result.IsValid.Should().BeFalse();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
            result.IsValid.Should().BeFalse();

            //validation without regex
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "test_phone_number" });
            result.IsValid.Should().BeFalse();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
            result.IsValid.Should().BeFalse();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "123" });
            result.IsValid.Should().BeFalse();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "[0-9]{1,14}^" });
            result.IsValid.Should().BeTrue();

            //validation with regex
            _userSettings.PhoneNumberValidationUseRegex = true;
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "test_phone_number" });
            result.IsValid.Should().BeFalse();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "123456789" });
            result.IsValid.Should().BeTrue();
            _userSettings.SmartPhoneRequired = false;
            result = await _validator.ValidateAsync(new Person { PhoneNumber = string.Empty });
            result.IsValid.Should().BeTrue();
            result = await _validator.ValidateAsync(new Person { PhoneNumber = "+123456789" });
            result.IsValid.Should().BeFalse();
        }
    }
}
