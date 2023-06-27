using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace ProyectoIdentity.Servicios
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        public AuthMessageSenderOptions Options { get; } //Se debe quitar?

        private readonly IConfiguration _configuration;


        public MailJetEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<MailJetEmailSender> logger, IConfiguration configuration)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var sendGridKey = _configuration["SendGridKey"];

            if (string.IsNullOrEmpty(sendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(sendGridKey, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("vladimir.cortes@heracomputo.com", "Password Recovery"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }
    }
}