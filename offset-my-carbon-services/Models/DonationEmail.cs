using System;

namespace offset_my_carbon_services.Models
{
    public class DonationEmail
    {
        public string Charity { get; set; }
        public string Referrer { get; set; }
        public int Trees { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
