using System.Net;
using System.Net.Mail;
using Chonicle.Services;

namespace Chronicle.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _configuration["Email:Host"]
            ?? throw new InvalidOperationException("Email:Host is not configured.");
        var portValue = _configuration["Email:Port"]
            ?? throw new InvalidOperationException("Email:Port is not configured.");
        var from = _configuration["Email:From"]
            ?? throw new InvalidOperationException("Email:From is not configured.");
        var user = _configuration["Email:User"]
            ?? throw new InvalidOperationException("Email:User is not configured.");
        var pass = _configuration["Email:Pass"]
            ?? throw new InvalidOperationException("Email:Pass is not configured.");

        var port = int.Parse(portValue);

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        using var message = new MailMessage
        {
            From = new MailAddress(from),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}