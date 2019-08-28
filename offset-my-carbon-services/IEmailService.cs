using offset_my_carbon_dal.Models;
using offset_my_carbon_services.Models;
using System.Collections.Generic;

namespace offset_my_carbon_services
{
    public interface IEmailService
    {
        void SendDonationEmail(DonationEmail donation);
        void SendWeeklyEmail(List<DonationEmail> donation, string charity);
    }
}
