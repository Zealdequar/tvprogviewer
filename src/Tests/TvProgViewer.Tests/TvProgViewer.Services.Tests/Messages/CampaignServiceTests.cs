﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Data;
using TvProgViewer.Services.Messages;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Messages
{
    [TestFixture]
    public class CampaignServiceTests : BaseTvProgTest
    {
        private ICampaignService _campaignService;
        private IRepository<Campaign> _campaignRepository;
        private IRepository<QueuedEmail> _cueuedEmailRepository;

        [OneTimeSetUp]
        public void SetUp()
        {
            _campaignService = GetService<ICampaignService>();
            _campaignRepository = GetService<IRepository<Campaign>>();
            _cueuedEmailRepository = GetService<IRepository<QueuedEmail>>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body"
            };

            var updateItem = new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body"
            };

            await TestCrud(insertItem, _campaignService.InsertCampaignAsync, updateItem, _campaignService.UpdateCampaignAsync, _campaignService.GetCampaignByIdAsync, (item, other) => item.Subject.Equals(other.Subject) && item.Body.Equals(other.Body) && item.Name.Equals(other.Name), _campaignService.DeleteCampaignAsync);
        }

        [Test]
        public async Task CanGetAllCampaigns()
        {
            var rez = await _campaignService.GetAllCampaignsAsync();
            rez.Count.Should().Be(0);
            await _campaignService.InsertCampaignAsync(new Campaign
            {
                Name = "Test name", Subject = "Test subject", Body = "Test body"
            });
            await _campaignService.InsertCampaignAsync(new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body",
                StoreId = 2
            });
            await _campaignService.InsertCampaignAsync(new Campaign
            {
                Name = "Test name",
                Subject = "Test subject",
                Body = "Test body",
                StoreId = 1
            });

            rez = await _campaignService.GetAllCampaignsAsync();
            rez.Count.Should().Be(3);
            rez = await _campaignService.GetAllCampaignsAsync(1);
            rez.Count.Should().Be(1);
            rez = await _campaignService.GetAllCampaignsAsync(2);
            rez.Count.Should().Be(1);
        }

        [Test]
        public async Task CanSendCampaign()
        {
            await _campaignRepository.TruncateAsync();
            var campaign = new Campaign {Name = "Test name", Subject = "Test subject", Body = "Test body"};
            await _campaignService.InsertCampaignAsync(campaign);
            TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;
            await _cueuedEmailRepository.TruncateAsync();

            var emailAccount = new EmailAccount
            {
                Id = 1,
                Email = TvProgTestsDefaults.AdminEmail,
                DisplayName = "Test name",
                Host = "smtp.test.com",
                Port = 25,
                Username = "test_user",
                Password = "test_password",
                EnableSsl = false,
                UseDefaultCredentials = false
            };

            var subscription = new NewsLetterSubscription { Active = true, Email = "test@test.com" };

            await _campaignService.SendCampaignAsync(campaign, emailAccount, new[] {subscription});
            _cueuedEmailRepository.Table.Count().Should().Be(1);

            await _campaignService.SendCampaignAsync(campaign, emailAccount, TvProgTestsDefaults.AdminEmail);
            TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
        }
    }
}
