using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Helpers;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests : ServiceTest
    {
        private IUserService _userService;
        private IGenericAttributeService _genericAttributeService;
        private DateTimeSettings _dateTimeSettings;
        private IDateTimeHelper _dateTimeHelper;
        private ISettingService _settingService;
        private User _user;

        /// <summary>
        /// (GMT+02:00) Minsk
        /// </summary>
        private string _gmtPlus2MinskGmtZone;
       
        /// <summary>
        /// (GMT+03:00) Moscow, St. Petersburg, Volgograd
        /// </summary>
        private string _gmtPlus3MoscowGmtZone;

        /// <summary>
        /// (GMT+07:00) Krasnoyarsk
        /// </summary>
        private string _gmtPlus7KrasnoyarskGmtZone;

        private string _defaultTimeZone;
        private bool _defaultAllowUsersToSetTimeZone;
        private string _defaultDefaultStoreGmtZone;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _userService = GetService<IUserService>();
            _genericAttributeService = GetService<IGenericAttributeService>();
            _dateTimeSettings = GetService<DateTimeSettings>();
            _dateTimeHelper = GetService<IDateTimeHelper>();
            _settingService = GetService<ISettingService>();

            _user = await GetService<IUserService>().GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);

            _defaultTimeZone =_user.GmtZone;

            _defaultAllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            _defaultDefaultStoreGmtZone = _dateTimeSettings.DefaultStoreGmtZone;

            _gmtPlus2MinskGmtZone = "E. Europe Standard Time";  //(GMT+02:00) Minsk
            _gmtPlus3MoscowGmtZone = "Russian Standard Time"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            _gmtPlus7KrasnoyarskGmtZone = "North Asia Standard Time"; //(GMT+07:00) Krasnoyarsk;

            if (Environment.OSVersion.Platform != PlatformID.Unix) 
                return;

            _gmtPlus2MinskGmtZone = "Europe/Minsk";  //(GMT+02:00) Minsk;
            _gmtPlus3MoscowGmtZone = "Europe/Moscow"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
            _gmtPlus7KrasnoyarskGmtZone = "Asia/Krasnoyarsk"; //(GMT+07:00) Krasnoyarsk;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _user.GmtZone = _defaultTimeZone;
            _dateTimeSettings.AllowUsersToSetTimeZone = _defaultAllowUsersToSetTimeZone;
            _dateTimeSettings.DefaultStoreGmtZone = _defaultDefaultStoreGmtZone;

            await _settingService.SaveSettingAsync(_dateTimeSettings);
        }

        [Test]
        public void CanGetAllSystemTimeZones()
        {
            var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
            systemTimeZones.Should().NotBeNull();
            systemTimeZones.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanGetUserTimeZoneWithCustomTimeZonesEnabled()
        {
            _dateTimeSettings.AllowUsersToSetTimeZone = true;
            _dateTimeSettings.DefaultStoreGmtZone = _gmtPlus2MinskGmtZone; //(GMT+02:00) Minsk;
            await _settingService.SaveSettingAsync(_dateTimeSettings);

            _user.GmtZone = _gmtPlus3MoscowGmtZone;
            await _userService.UpdateUserAsync(_user);

            var timeZone = await GetService<IDateTimeHelper>().GetUserTimeZoneAsync(_user);
            timeZone.Should().NotBeNull();
            timeZone.Id.Should().Be(_gmtPlus3MoscowGmtZone);
        }

        [Test]
        public async Task CanGetUserTimezoneWithCustomTimeZonesDisabled()
        {
            _dateTimeSettings.AllowUsersToSetTimeZone = false;
            _dateTimeSettings.DefaultStoreGmtZone = _gmtPlus2MinskGmtZone; //(GMT+02:00) Minsk;
            await _settingService.SaveSettingAsync(_dateTimeSettings);

            _user.GmtZone = _gmtPlus3MoscowGmtZone;
            await _userService.UpdateUserAsync(_user);
            
            var timeZone = await GetService<IDateTimeHelper>().GetUserTimeZoneAsync(_user);
            timeZone.Should().NotBeNull();
            timeZone.Id.Should().Be(_gmtPlus2MinskGmtZone);  //(GMT+02:00) Minsk
        }

        [Test]
        public void CanConvertDatetimeToUserTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskGmtZone); //(GMT+02:00) Minsk;
            sourceDateTime.Should().NotBeNull();

            var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus7KrasnoyarskGmtZone); //(GMT+07:00) Krasnoyarsk;
            destinationDateTime.Should().NotBeNull();

            //summer time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 06, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .Should().Be(new DateTime(2010, 06, 01, 5, 0, 0));

            //winter time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 01, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .Should().Be(new DateTime(2010, 01, 01, 5, 0, 0));
        }

        [Test]
        public void CanConvertDatetimeToUtcDateTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskGmtZone); //(GMT+02:00) Minsk;
            sourceDateTime.Should().NotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
            convertedDateTime1.Should().Be(new DateTime(2010, 05, 31, 21, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
            convertedDateTime2.Should().Be(new DateTime(2009, 12, 31, 22, 0, 0));
        }
    }
}
