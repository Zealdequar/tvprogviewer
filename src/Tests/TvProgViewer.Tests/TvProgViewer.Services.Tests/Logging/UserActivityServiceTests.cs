using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Logging;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Logging
{
    [TestFixture]
    public class UserActivityServiceTests : ServiceTest
    {
        private IUserActivityService _userActivityService;
        private IUserService _userService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _userActivityService = GetService<IUserActivityService>();
            _userService = GetService<IUserService>();
        }

        [Test]
        public async Task CanFindActivities()
        {
            var user = await _userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);

            var activities = await _userActivityService.GetAllActivitiesAsync(userId: user.Id, pageSize: 10);
            activities.Any().Should().BeTrue();

            user = await _userService.GetUserByEmailAsync("builtin@search_engine_record.com");

            activities = await _userActivityService.GetAllActivitiesAsync(userId: user.Id, pageSize: 10);
            activities.Any().Should().BeFalse();
        }
    }
}
