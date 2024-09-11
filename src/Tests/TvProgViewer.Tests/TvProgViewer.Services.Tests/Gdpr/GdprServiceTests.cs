using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Services.Gdpr;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Gdpr
{
    [TestFixture]
    public class GdprServiceTests : BaseTvProgTest
    {
        private IGdprService _gdprService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _gdprService = GetService<IGdprService>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new GdprConsent
            {
                Message = "Test message"
            };

            var updateItem = new GdprConsent
            {
                Message = "Update test message"
            };

            await TestCrud(insertItem, _gdprService.InsertConsentAsync, updateItem, _gdprService.UpdateConsentAsync, _gdprService.GetConsentByIdAsync, (item, other) => item.Message.Equals(other.Message), _gdprService.DeleteConsentAsync);
        }
    }
}
