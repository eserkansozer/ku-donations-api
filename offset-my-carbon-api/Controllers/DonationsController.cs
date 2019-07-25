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

namespace offset_my_carbon_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationsRepository _repository;

        public DonationsController(IDonationsRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Donations
        [HttpGet]
        public IEnumerable<Donation> GetDonations()
        {
            return _repository.GetDonations();
        }

        // GET: api/Donations/test
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

        //// POST: api/Donations
        //[HttpPost]
        //public async Task<IActionResult> PostDonation([FromBody] Donation donation)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _repository.Donations.Add(donation);
        //    await _repository.SaveChangesAsync();

        //    return CreatedAtAction("GetDonation", new { id = donation.Id }, donation);
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