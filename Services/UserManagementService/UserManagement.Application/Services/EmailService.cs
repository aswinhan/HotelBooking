using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = _configuration["EmailSettings:SmtpPort"];
        var smtpUser = _configuration["EmailSettings:SmtpUser"];
        var smtpPass = _configuration["EmailSettings:SmtpPass"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];

        if (smtpHost == null || smtpPort == null || smtpUser == null || smtpPass == null || senderEmail == null)
        {
            throw new InvalidOperationException("Email settings are not configured properly.");
        }

        var smtpClient = new SmtpClient(smtpHost)
        {
            Port = int.Parse(smtpPort),
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);
        await smtpClient.SendMailAsync(mailMessage);
    }
}
