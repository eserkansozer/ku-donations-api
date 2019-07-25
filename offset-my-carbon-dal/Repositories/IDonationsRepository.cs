using System;
using System.Collections.Generic;
using System.Text;

namespace offset_my_carbon_dal.Repositories
{
    public interface IDonationsRepository
    {
        IEnumerable<Models.Donation> GetDonations();
        IEnumerable<Models.Donation> GetDonationsByCharity(string charity);
    }
}
