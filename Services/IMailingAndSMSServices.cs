using Twilio.Rest.Api.V2010.Account;

namespace Project.Services
{
    public interface IMailingAndSMSServices
    {
        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
        Task<MessageResource>  SendSMSAsync(string mobileNumber, string body);
    }
}
