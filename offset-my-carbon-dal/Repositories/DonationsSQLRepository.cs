using System;
using System.Collections.Generic;
using System.Linq;
using offset_my_carbon_dal.Data;
using offset_my_carbon_dal.Models;

namespace offset_my_carbon_dal.Repositories
{
    public class DonationsSqlRepository : IDonationsRepository
    {
        private readonly DataContext _dataContext;

        public DonationsSqlRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<Donation> GetDonations()
        {
            return _dataContext.Donations;
        }

        public IEnumerable<Donation> GetDonationsByCharity(string charity)
        {
            return _dataContext.Donations.Where(d => d.Charity == charity);
        }
    }
}
