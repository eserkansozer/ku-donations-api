using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using offset_my_carbon_dal.Models;
using offset_my_carbon_dal.Repositories;
using offset_my_carbon_services.Models;

namespace offset_my_carbon_api.Controllers
{
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationsRepository _repository;

        public IConfiguration Configuration { get; }

        public DonationsController(IDonationsRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            Configuration = configuration;
        }

        // GET: api/Donations
        [Route("api/donations")]
        [HttpGet]
        public IEnumerable<Donation> GetDonations()
        {
            return _repository.GetDonations();
        }

        // GET: api/Donations/count
        [Route("api/donations/count")]
        [HttpGet]
        public int GetDonationCount()
        {
            return _repository.GetDonations().Count();
        }

        // GET: api/Donations/trees/count
        [Route("api/donations/trees/count")]
        [HttpGet]
        public int GetDonationTreeCount()
        {
            return _repository.GetDonations().Sum(d => d.Trees);
        }

        // GET: api/Donations/test/count
        [Route("api/donations/{charity}/count")]
        [HttpGet("{charity}")]
        public int GetDonationCountForCharity([FromRoute] string charity)
        {
            return _repository.GetDonationsByCharity(charity).Count();
        }

        // GET: api/Donations/test/trees/count
        [Route("api/donations/{charity}/trees/count")]
        [HttpGet("{charity}")]
        public int GetDonationTreeCountForCharity([FromRoute] string charity)
        {
            return _repository.GetDonationsByCharity(charity).Sum(d => d.Trees);
        }

        // GET: api/Donations/test
        [Route("api/donations/{charity}")]
        [HttpGet("{charity}")]
        public IActionResult GetDonation([FromRoute] string charity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donation = _repository.GetDonationsByCharity(charity);

            if (donation == null)
            {
                return NotFound();
            }

            return Ok(donation);
        }

        // POST: api/Donations
        [Route("api/donations")]
        [HttpPost]
        public IActionResult PostDonation([FromBody] Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            donation.TimeStamp = DateTime.Now;
            _repository.AddDonation(donation);

            AddDonationToEmailQueue(donation);

            return CreatedAtAction("GetDonation", new { id = donation.Id }, donation);
        }

        // GET: api/Donations/5
        [HttpGet("api/donation/{id}")]
        public IActionResult GetDonation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donation = _repository.GetDonationById(id);

            if (donation == null)
            {
                return NotFound();
            }

            return Ok(donation);
        }

        [HttpGet("api/weeklydonations/{charity}")]
        public IActionResult WeeklyDonations(string charity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donations = _repository.GetDonationsByCharity(charity);
            var weeksDonations = donations.Where(d => d.TimeStamp >= DateTime.Now.AddDays(-7));
            return new JsonResult(weeksDonations);
        }

        [HttpGet("api/monthlydonations/{charity}")]
        public IActionResult MonthlyDonations(string charity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donations = _repository.GetDonationsByCharity(charity);
            var monthsDonations = donations.Where(d => d.TimeStamp >= DateTime.Now.AddMonths(-1));
            return new JsonResult(monthsDonations);
        }

        private void AddDonationToEmailQueue(Donation donation)
        {
            var donationEmail = new DonationEmail()
            {
                Charity = donation.Charity,
                Referrer = donation.Referrer,
                TimeStamp = donation.TimeStamp,
                Trees = donation.Trees
            };

            string storageConnectionString = Configuration.GetConnectionString("StorageConnectionString");

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // Create the CloudQueueClient that represents the queue endpoint for the storage account.
                CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();

                var queue = cloudQueueClient.GetQueueReference("donationemailsqueue");

                var jsonMessage = JsonConvert.SerializeObject(donationEmail);
                CloudQueueMessage message = new CloudQueueMessage(jsonMessage);
                queue.AddMessage(message, new TimeSpan(7, 0, 0, 0), null, null, null);
            }
        }

        //[HttpGet("api/weeklyreport/{charity}")]
        //public IActionResult WeeklyReport(string charity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var donations = _repository.GetDonationsByCharity(charity);
        //    var weeksDonations = donations.Where(d => d.TimeStamp >= DateTime.Now.AddDays(-7));
        //    _emailService.SendWeeklyEmail(weeksDonations.ToList(), "egeorman");           

        //    return Ok();
        //}

        //// PUT: api/Donations/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDonation([FromRoute] int id, [FromBody] Donation donation)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != donation.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _repository.Entry(donation).State = EntityState.Modified;

        //    try
        //    {
        //        await _repository.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DonationExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// DELETE: api/Donations/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDonation([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var donation = await _repository.Donations.FindAsync(id);
        //    if (donation == null)
        //    {
        //        return NotFound();
        //    }

        //    _repository.Donations.Remove(donation);
        //    await _repository.SaveChangesAsync();

        //    return Ok(donation);
        //}

        //private bool DonationExists(int id)
        //{
        //    return _repository.Donations.Any(e => e.Id == id);
        //}
    }
}