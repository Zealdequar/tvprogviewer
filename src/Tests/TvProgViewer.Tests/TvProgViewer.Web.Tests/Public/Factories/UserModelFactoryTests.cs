﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Users;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.User;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class UserModelFactoryTests : WebTest
    {
        private IUserModelFactory _userModelFactory;
        private User _user;
        private IUserAttributeService _userAttributeService;
        private UserAttribute[] _userAttributes;


        [OneTimeSetUp]
        public async Task SetUp()
        {
            _userModelFactory = GetService<IUserModelFactory>();
            _user = await GetService<IWorkContext>().GetCurrentUserAsync();

            _userAttributeService = GetService<IUserAttributeService>();

            _userAttributes = new[]
            {
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.Checkboxes, Name = "Test user attribute 1"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.ColorSquares, Name = "Test user attribute 2"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.Datepicker, Name = "Test user attribute 3"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.DropdownList, Name = "Test user attribute 4"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.FileUpload, Name = "Test user attribute 5"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.ImageSquares, Name = "Test user attribute 6"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.MultilineTextbox, Name = "Test user attribute 7"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.RadioList, Name = "Test user attribute 8"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.ReadonlyCheckboxes, Name = "Test user attribute 9"
                },
                new UserAttribute
                {
                    AttributeControlType = AttributeControlType.TextBox, Name = "Test user attribute 10"
                }
            };

            foreach (var userAttribute in _userAttributes) 
                await _userAttributeService.InsertUserAttributeAsync(userAttribute);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            foreach (var userAttribute in _userAttributes)
                await _userAttributeService.DeleteUserAttributeAsync(userAttribute);
        }

        [Test]
        public async Task CanPrepareUserInfoModel()
        {
            var model = await _userModelFactory.PrepareUserInfoModelAsync(new UserInfoModel(), _user, false);
            model.AvailableTimeZones.Any().Should().BeTrue();

            model.Email.Should().Be(TvProgTestsDefaults.AdminEmail);
            model.Username.Should().Be(TvProgTestsDefaults.AdminEmail);
            model.FirstName.Should().Be("John");
            model.LastName.Should().Be("Smith");

            model = await _userModelFactory.PrepareUserInfoModelAsync(new UserInfoModel(), _user, true);

            model.Email.Should().BeNullOrEmpty();
            model.Username.Should().BeNullOrEmpty();
            model.FirstName.Should().BeNullOrEmpty();
            model.LastName.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task CanPrepareRegisterModel()
        {
            var model = await _userModelFactory.PrepareRegisterModelAsync(new RegisterModel(), false);
            model.AvailableTimeZones.Any().Should().BeTrue();
            model.UserAttributes.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanPrepareLoginModel()
        {
            var model = await _userModelFactory.PrepareLoginModelAsync(null);
            model.CheckoutAsGuest.Should().Be(default);
            model = await _userModelFactory.PrepareLoginModelAsync(true);
            model.CheckoutAsGuest.Should().BeTrue();
            model = await _userModelFactory.PrepareLoginModelAsync(false);
            model.CheckoutAsGuest.Should().BeFalse();
        }

        [Test]
        public async Task CanPreparePasswordRecoveryModel()
        {
            var model = await _userModelFactory.PreparePasswordRecoveryModelAsync(new PasswordRecoveryModel{Email = "test@email.com"});
            model.DisplayCaptcha.Should().BeFalse();
            model.Email.Should().Be("test@email.com");
        }
        
        [Test]
        public async Task CanPrepareRegisterResultModel()
        {
            var model = await _userModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.AdminApproval, string.Empty);
            model.Result.Should().Be("Your account will be activated after approving by administrator.");
            model = await _userModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Disabled, string.Empty);
            model.Result.Should().Be("Registration not allowed. You can edit this in the admin area.");
            model = await _userModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.EmailValidation, string.Empty);
            model.Result.Should().Be("Your registration has been successfully completed. You have just been sent an email containing activation instructions.");
            model = await _userModelFactory.PrepareRegisterResultModelAsync((int)UserRegistrationType.Standard, string.Empty);
            model.Result.Should().Be("Your registration completed");
            model = await _userModelFactory.PrepareRegisterResultModelAsync(400, string.Empty);
            model.Result.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task CanPrepareCustomUserAttributes()
        {
            var model = await _userModelFactory.PrepareCustomUserAttributesAsync(_user);
            model.Any().Should().BeTrue();
            model.Count.Should().Be(10);
        }
    }
}
