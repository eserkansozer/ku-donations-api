using offset_my_carbon_dal.Models;

namespace Services
{
    public interface IEmailService
    {
        void SendDonationEmail(Donation donation);
    }
}
