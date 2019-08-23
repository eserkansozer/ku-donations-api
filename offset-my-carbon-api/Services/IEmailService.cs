using offset_my_carbon_dal.Models;
using System.Collections.Generic;

namespace Services
{
    public interface IEmailService
    {
        void SendDonationEmail(Donation donation);
        void SendWeeklyEmail(List<Donation> donation, string charity);
    }
}
