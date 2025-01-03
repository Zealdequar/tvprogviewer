﻿using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Services.Messages;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Messages
{
    [TestFixture]
    public class EmailSenderTests : BaseTvProgTest
    {
        private IEmailSender _emailSender;

        [OneTimeSetUp]
        public void SetUp()
        {
            _emailSender = GetService<IEmailSender>();
        }

        [Test]
        public async Task CanSendEmail()
        {
            TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;

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

            var subject = "Test subject";
            var body = "Test body";
            var fromAddress = TvProgTestsDefaults.AdminEmail;
            var fromName = "From name";
            var toAddress = "test@test.com";
            var toName = "To name";
            var replyToAddress = TvProgTestsDefaults.AdminEmail;
            var replyToName = "Reply to name";
            var bcc = new[] {TvProgTestsDefaults.AdminEmail};
            var cc = new[] { TvProgTestsDefaults.AdminEmail };

            await _emailSender.SendEmailAsync(emailAccount, subject, body,
                fromAddress, fromName, toAddress, toName,
                replyToAddress, replyToName, bcc, cc);

            TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
        }
    }
}
