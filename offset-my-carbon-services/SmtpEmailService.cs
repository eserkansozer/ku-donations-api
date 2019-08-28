using System;
using Microsoft.Extensions.Configuration;
using offset_my_carbon_dal.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using offset_my_carbon_services.Models;

namespace offset_my_carbon_services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendDonationEmail(DonationEmail donation)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress("KarbonsuzUcus.web", _config.GetValue<string>("SMTP:WebEmail"));
            message.From.Add(from);

            var to = new MailboxAddress("KarbonsuzUcus.admin", _config.GetValue<string>("SMTP:AdminEmail"));
            message.To.Add(to);

            message.Subject = $"{donation.Charity} vakfina bagis yonlendirildi: {donation.Trees} agac!";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<strong>KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!</strong><ul>" +
                              $"<li>Tarih: {donation.TimeStamp.ToShortDateString()}</li>" +
                              $"<li>Vakif: {donation.Charity}</li>" +
                              $"<li>Adet: {donation.Trees}</li>" +
                              $"<li>Kaynak: {(String.IsNullOrEmpty(donation.Referrer) ? ("Diger") : donation.Referrer)}</li>" +
                              $"</ul>";
            bodyBuilder.TextBody = "KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!";

            message.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient();
            var smtpAddress = _config.GetValue<string>("SMTP:Address");
            var smtpPort = _config.GetValue<int>("SMTP:Port");
            var smtpUserName = _config.GetValue<string>("SMTP:UserName");
            var smtpPassword = _config.GetValue<string>("SMTP:Password");
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(smtpAddress, smtpPort, true);
            client.Authenticate(smtpUserName, smtpPassword);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }

        public void SendWeeklyEmail(List<DonationEmail> donations, string charity)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress("KarbonsuzUcus.web", _config.GetValue<string>("SMTP:WebEmail"));
            message.From.Add(from);

            var toAdmin = new MailboxAddress($"KarbonsuzUcus.{charity}", _config.GetValue<string>("SMTP:AdminEmail"));
            message.To.Add(toAdmin);

            var toCharity = new MailboxAddress($"KarbonsuzUcus.{charity}", _config.GetValue<string>("SMTP:CharityEmail"));
            message.To.Add(toCharity);

            message.Subject = $"{charity} haftalik rapor: Toplam {donations.Sum(d=>d.Trees)} agac bagisi";

            var sb = new StringBuilder();
            sb.Append($"<strong>KarbonsuzUcus.com araciligiyla yonlendirilen agac bagislari</strong><ul>");
            foreach (var donation in donations)
            {
                sb.Append($"<li>Tarih: {donation.TimeStamp.ToShortDateString()}, Adet: {donation.Trees}, Kaynak: {(String.IsNullOrEmpty(donation.Referrer) ? ("Diger") : donation.Referrer)}</li>");
            }
            sb.Append($"</ul>");

            var bodyBuilder = new BodyBuilder();

            bodyBuilder.TextBody = $"{charity} haftalik rapor: Toplam {donations.Count} agac bagisi";
            bodyBuilder.HtmlBody = sb.ToString();
            message.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient();
            var smtpAddress = _config.GetValue<string>("SMTP:Address");
            var smtpPort = _config.GetValue<int>("SMTP:Port");
            var smtpUserName = _config.GetValue<string>("SMTP:UserName");
            var smtpPassword = _config.GetValue<string>("SMTP:Password");
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(smtpAddress, smtpPort, true);
            client.Authenticate(smtpUserName, smtpPassword);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
