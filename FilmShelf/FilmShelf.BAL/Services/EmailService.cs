using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace FilmShelf.BAL.Services;

public class EmailService : IEmailService
{
    private readonly MailJetSettings _mailJetSettings;

    public EmailService(IOptions<MailJetSettings> mailJetSettings)
    {
        _mailJetSettings = mailJetSettings.Value;
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.ApiSecret);

        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, _mailJetSettings.FromEmail)
        .Property(Send.FromName, _mailJetSettings.FromName)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, body)
        .Property(Send.Recipients, new JArray { new JObject { { "Email", toEmail } } });

        await client.PostAsync(request);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "Password Reset Request";
        var body = $"Please reset your password by clicking on this link: <a href='{resetLink}'>Reset Password</a>";
        await SendEmailAsync(toEmail, subject, body);
    }
}
