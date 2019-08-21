using System;

namespace offset_my_carbon_dal.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string Charity { get; set; }
        public string Referrer { get; set; }
        public int Trees { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
