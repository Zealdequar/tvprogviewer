using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Services.Messages;

namespace TvProgViewer.Tests
{
    public class TestSmtpBuilder : SmtpBuilder
    {
        public TestSmtpBuilder(EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService) : base(emailAccountSettings, emailAccountService)
        {
        }

        public override Task<SmtpClient> BuildAsync(EmailAccount emailAccount = null)
        {
            return Task.FromResult<SmtpClient>(new TestSmtpClient());
        }

        public class TestSmtpClient : SmtpClient
        {
            public override Task<string> SendAsync(MimeMessage message,
                CancellationToken cancellationToken = default,
                ITransferProgress progress = null)
            {
                MessageIsSent = true;
                return Task.FromResult(string.Empty);
            }

            public static bool MessageIsSent { get; set; }
        }
    }
}
