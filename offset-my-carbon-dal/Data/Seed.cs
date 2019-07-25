using System.Linq;

namespace offset_my_carbon_dal.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedDonations()
        {
            if (_context.Donations.Any())
            {
                return;
            }

            _context.Donations.Add(new Models.Donation { Charity="TestCharity", Referrer = "TestReferrer", Trees = 1 });
            _context.Donations.Add(new Models.Donation { Charity="TestCharity", Referrer = "TestReferrer", Trees = 5 });
            _context.Donations.Add(new Models.Donation { Charity="TestCharity", Referrer = "TestReferrer", Trees = 10 });

            _context.SaveChanges();
        }
    }
}
