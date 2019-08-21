using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using offset_my_carbon_dal.Data;
using offset_my_carbon_dal.Models;
using offset_my_carbon_dal.Repositories;
using Services;

namespace offset_my_carbon_api.Controllers
{

    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationsRepository _repository;
        private readonly IEmailService _emailService;

        public DonationsController(IDonationsRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
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

            try
            {
                _emailService.SendDonationEmail(donation);
            }catch(Exception) { };

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