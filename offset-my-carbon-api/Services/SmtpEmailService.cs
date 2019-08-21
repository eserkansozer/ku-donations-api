using System;
using Microsoft.Extensions.Configuration;
using offset_my_carbon_dal.Models;

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
            var apiKey = _config.GetValue<string>("SendGrid:SendGridApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("karbonsuzucus@groovytechs.co.uk", "KarbonsuzUcus.com - web");
            var subject = $"Bagis yonlendirildi: {donation.Trees} agac!";
            var to = new EmailAddress("karbonsuzucus@groovytechs.co.uk", "KarbonsuzUcus.com - admin");
            var plainTextContent = "KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!";
            var htmlContent = $"<strong>KarbonsuzUcus.com araciligiyla bir bagis daha yonlendirildi!</strong><ul>" +
                              $"<li>Tarih: {donation.TimeStamp.ToShortDateString()}</li>" +
                              $"<li>Vakif: {donation.Charity}</li>" +
                              $"<li>Adet: {donation.Trees}</li>" +
                              $"<li>Kaynak: {donation.Referrer}</li>" +
                              $"</ul>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
            response.Wait();
        }
    }
}
