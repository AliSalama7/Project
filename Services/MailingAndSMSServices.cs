using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Project.Helpers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Project.Services
{
    public class MailingAndSMSServices : IMailingAndSMSServices
    {
        private readonly MailSettings _mailSettings;
        private readonly TwilioSettings _twilioSettings;

        public MailingAndSMSServices(IOptions<MailSettings> mailSettings, IOptions<TwilioSettings> twilioSettings)
        {
            _mailSettings = mailSettings.Value;
            _twilioSettings = twilioSettings.Value;
        }

        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject
            };

            email.To.Add(MailboxAddress.Parse(mailTo));

            var builder = new BodyBuilder();

            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

            using SmtpClient smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public Task<MessageResource> SendSMSAsync(string mobileNumber, string body)
        {
            TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);

            var result = MessageResource.CreateAsync(
                         body : body,
                         from : new Twilio.Types.PhoneNumber(_twilioSettings.TwilioPhoneNumber),
                         to : mobileNumber
                );

            return result;
        }
    }
}
