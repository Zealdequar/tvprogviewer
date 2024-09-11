using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.WebUI.Factories;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class NewsletterModelFactoryTests : BaseTvProgTest
    {
        private INewsletterModelFactory _newsletterModelFactory;

        [OneTimeSetUp]
        public void SetUp()
        {
            _newsletterModelFactory = GetService<INewsletterModelFactory>();
        }

        [Test]
        public async Task CanPrepareNewsletterBoxModel()
        {
            var model = await _newsletterModelFactory.PrepareNewsletterBoxModelAsync();

            model.AllowToUnsubscribe.Should().Be(GetService<UserSettings>().NewsletterBlockAllowToUnsubscribe);
        }

        [Test]
        public async Task CanPrepareSubscriptionActivationModel()
        {
            var activated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(true)).Result;

            var deactivated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(false)).Result;

            activated.Should().NotBe(deactivated);
        }
    }
}
