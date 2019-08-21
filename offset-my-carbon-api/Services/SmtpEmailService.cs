using System;
using Microsoft.Extensions.Configuration;
using offset_my_carbon_dal.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendDonationEmail(Donation donation)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress("KarbonsuzUcus.web",
            "karbonsuzucus@groovytechs.co.uk");
            message.From.Add(from);

            var to = new MailboxAddress("KarbonsuzUcus.admin",
            "karbonsuzucus@groovytechs.co.uk");
            message.To.Add(to);

            message.Subject = $"{donation.Charity}'e bagis yonlendirildi: {donation.Trees} agac!";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<strong>KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!</strong><ul>" +
                              $"<li>Tarih: {donation.TimeStamp.ToShortDateString()}</li>" +
                              $"<li>Vakif: {donation.Charity}</li>" +
                              $"<li>Adet: {donation.Trees}</li>" +
                              $"<li>Kaynak: {donation.Referrer}</li>" +
                              $"</ul>";
            bodyBuilder.TextBody = "KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!";

            message.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient();
            var smtpAddress = _config.GetValue<string>("SMTP:Address");
            var smtpPort = _config.GetValue<int>("SMTP:Port");
            var smtpUserName = _config.GetValue<string>("SMTP:UserName");
            var smtpPassword = _config.GetValue<string>("SMTP:Password");
            client.ServerCertificateValidationCallback = (s, c, h, e) => {
                return ((System.Security.Cryptography.X509Certificates.X509Certificate2)c).Thumbprint == _config.GetValue<string>("SMTP:Thumbprint");
            };
            client.Connect(smtpAddress, smtpPort, true);
            client.Authenticate(smtpUserName, smtpPassword);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
