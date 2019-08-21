using offset_my_carbon_dal.Models;
using System.Collections.Generic;

namespace offset_my_carbon_dal.Repositories
{
    public interface IDonationsRepository
    {
        Donation GetDonationById(int id);
        IEnumerable<Donation> GetDonations();
        IEnumerable<Donation> GetDonationsByCharity(string charity);
        void AddDonation(Donation donation);
    }
}
