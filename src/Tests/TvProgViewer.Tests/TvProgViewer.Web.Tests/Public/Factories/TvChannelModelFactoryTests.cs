using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class TvChannelModelFactoryTests : WebTest
    {
        private ITvChannelModelFactory _tvChannelModelFactory;
        private ITvChannelService _tvChannelService;
        private IUrlRecordService _urlRecordService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _tvChannelModelFactory = GetService<ITvChannelModelFactory>();
            _tvChannelService = GetService<ITvChannelService>();
            _urlRecordService = GetService<IUrlRecordService>();
        }

        [Test]
        public async Task CanPrepareTvChannelTemplateViewPath()
        {
            var tvChannelTemplateRepository = GetService<IRepository<TvChannelTemplate>>();
            var tvChannelTemplateSimple = tvChannelTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple tvChannel");
            if (tvChannelTemplateSimple == null)
                throw new Exception("Simple tvChannel template could not be loaded");
            var tvChannelTemplateGrouped = tvChannelTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped tvChannel (with variants)");
            if (tvChannelTemplateGrouped == null)
                throw new Exception("Grouped tvChannel template could not be loaded");

            var modelSimple = await _tvChannelModelFactory.PrepareTvChannelTemplateViewPathAsync(new TvChannel
            {
                TvChannelTemplateId = tvChannelTemplateSimple.Id
            });

            var modelGrouped = await _tvChannelModelFactory.PrepareTvChannelTemplateViewPathAsync(new TvChannel
            {
                TvChannelTemplateId = tvChannelTemplateGrouped.Id
            });

            modelSimple.Should().NotBe(modelGrouped);

            modelSimple.Should().Be(tvChannelTemplateSimple.ViewPath);
            modelGrouped.Should().Be(tvChannelTemplateGrouped.ViewPath);
        }

        [Test]
        public async Task CanPrepareTvChannelOverviewModels()
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            var model = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(new[] { tvChannel })).FirstOrDefault();

            PropertiesShouldEqual(tvChannel, model);
        }

        [Test]
        public async Task CanPrepareTvChannelDetailsModel()
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            var model = (await _tvChannelModelFactory.PrepareTvChannelOverviewModelsAsync(new[] { tvChannel })).FirstOrDefault();

            PropertiesShouldEqual(tvChannel, model);
        }

        [Test]
        public async Task CanPrepareTvChannelReviewsModel()
        {
            var pId = (await _tvChannelService.GetTvChannelReviewByIdAsync(1)).TvChannelId;
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(pId);
            var model = await _tvChannelModelFactory.PrepareTvChannelReviewsModelAsync(new TvChannelReviewsModel(), tvChannel);

            model.TvChannelId.Should().Be(tvChannel.Id);
            model.TvChannelName.Should().Be(tvChannel.Name);
            model.TvChannelSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(tvChannel));

            model.Items.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanPrepareUserTvChannelReviewsModel()
        {
            var model = await _tvChannelModelFactory.PrepareUserTvChannelReviewsModelAsync(null);
            var review = model.TvChannelReviews.FirstOrDefault();

            review.Should().NotBeNull();

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(review.TvChannelId);

            review.TvChannelName.Should().Be(tvChannel.Name);
            review.TvChannelSeName.Should().Be(await _urlRecordService.GetSeNameAsync(tvChannel));
        }

        [Test]
        public async Task CanPrepareTvChannelEmailAFriendModel()
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            var model = await _tvChannelModelFactory.PrepareTvChannelEmailAFriendModelAsync(new TvChannelEmailAFriendModel(), tvChannel, false);

            model.TvChannelId.Should().Be(tvChannel.Id);
            model.TvChannelName.Should().Be(tvChannel.Name);
            model.TvChannelSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(tvChannel));
            model.YourEmailAddress.Should().Be(TvProgTestsDefaults.AdminEmail);
        }

        [Test]
        public async Task CanPrepareTvChannelSpecificationModel()
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(1);
            var model = await _tvChannelModelFactory.PrepareTvChannelSpecificationModelAsync(tvChannel);

            var group = model.Groups.FirstOrDefault();

            group.Should().NotBe(null);
        }
    }
}
