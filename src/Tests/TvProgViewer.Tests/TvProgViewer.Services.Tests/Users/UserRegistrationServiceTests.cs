﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Security;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Users
{
    [TestFixture]
    public class UserRegistrationServiceTests : ServiceTest
    {
        private IUserService _userService;
        private IEncryptionService _encryptionService;
        private IUserRegistrationService _userRegistrationService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _userService = GetService<IUserService>();
            _encryptionService = GetService<IEncryptionService>();
            _userRegistrationService = GetService<IUserRegistrationService>();
        }

        private async Task<User> CreateUserAsync(PasswordFormat passwordFormat, bool isRegistered = true)
        {
            var user = new User
            {
                Username = "test@test.com",
                Email = "test@test.com",
                Active = true
            };

            await _userService.InsertUserAsync(user);

            var password = "password";
            if (passwordFormat == PasswordFormat.Encrypted)
                password = _encryptionService.EncryptText(password);

            await _userService.InsertUserPasswordAsync(new UserPassword
            {
                UserId = user.Id,
                PasswordFormat = passwordFormat,
                Password = password,
                CreatedOnUtc = DateTime.UtcNow
            });

            if (isRegistered)
            {
                var registeredRole = await _userService
                    .GetUserRoleBySystemNameAsync(TvProgUserDefaults.RegisteredRoleName);
                await _userService.AddUserRoleMappingAsync(new UserUserRoleMapping
                {
                    UserId = user.Id, UserRoleId = registeredRole.Id
                });
            }

            return user;
        }

        private async Task DeleteUserAsync(User user)
        {
            user.Username = user.Email = string.Empty;
            user.Active = false;
            await _userService.UpdateUserAsync(user);
            await _userService.DeleteUserAsync(user);
        }

        [Test]
        public async Task EnsureOnlyRegisteredUsersCanLogin()
        {
            var result = await _userRegistrationService.ValidateUserAsync(TvProgTestsDefaults.AdminEmail, TvProgTestsDefaults.AdminPassword);
            result.Should().Be(UserLoginResults.Successful);

            var user = await CreateUserAsync(PasswordFormat.Clear, false);

            result = await _userRegistrationService.ValidateUserAsync("test@test.com", "password");
            await DeleteUserAsync(user);

            result.Should().Be(UserLoginResults.NotRegistered);
        }

        [Test]
        public async Task CanValidateHashedPassword()
        {
            var result = await _userRegistrationService.ValidateUserAsync(TvProgTestsDefaults.AdminEmail, TvProgTestsDefaults.AdminPassword);
            result.Should().Be(UserLoginResults.Successful);
        }

        [Test]
        public async Task CanValidateClearPassword()
        {
            var user = await CreateUserAsync(PasswordFormat.Clear);

            var result = await _userRegistrationService.ValidateUserAsync("test@test.com", "password");
            await DeleteUserAsync(user);

            result.Should().Be(UserLoginResults.Successful);
        }

        [Test]
        public async Task CanValidateEncryptedPassword()
        {
            var user = await CreateUserAsync(PasswordFormat.Encrypted);

            var result = await _userRegistrationService.ValidateUserAsync("test@test.com", "password");
            await DeleteUserAsync(user);

            result.Should().Be(UserLoginResults.Successful);
        }
        
        [Test]
        public async Task CanChangePassword()
        {
            var user = await CreateUserAsync(PasswordFormat.Encrypted);

            var request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Clear, "password", "password");
            var unSuccess = await _userRegistrationService.ChangePasswordAsync(request);
            
            request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
            var success = await _userRegistrationService.ChangePasswordAsync(request);

            unSuccess.Success.Should().BeFalse();
            success.Success.Should().BeTrue();

            await DeleteUserAsync(user);
        }
    }
}
