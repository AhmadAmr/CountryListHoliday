using CountryListHoliday.Models.RemoteModels;
using CountryListHoliday.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace CountryListHoliday.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private ICountry _countryRepo;

        public CountryController(ICountry country)
        {
            _countryRepo = country;
        }

        [HttpGet("Sync")]
        public async Task<ActionResult> SyncCountries()
        {
            try
            {
                var countryList = await _countryRepo.GetAllCountriesAsync();
                await _countryRepo.GetAllHolidaiesASync();
                return Ok("All Countries Synced");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("ListAllCountries/{pageNumber}")]
        public async Task<List<Models.Country>> ListAllCountriesAsync(int pageNumber)
        {
            return await _countryRepo.ListAllCountriesAsync(pageNumber);

          
        }

        [HttpGet("ListHoliDays/{name}")]
        public async Task<List<Models.Holiday>> ListHoliDaysAsync(string name)
        {
            return await _countryRepo.GetHoliDays(name);


        }

        [HttpPost("AddHoliDay")]
        public async Task<ActionResult> AddHoliDay([FromBody] HoliDaySubmitModel model)
        {

            await _countryRepo.AddHoliyDay(model);

            return Ok("Holy Day Added");
        }


        [HttpPost("RemoveHolyDay")]
        public async Task<ActionResult> RemoveHolyDay([FromBody] string holidayID)
        {
            var result = await _countryRepo.RemoveHoliyDay(holidayID);

            if (result) return Ok("HolyDay Removed");
            return BadRequest("BadRequest");
        }


        [HttpPost("UpdateHoliDay")]
        public async Task<ActionResult> UpdateHoliDay([FromBody] HoliDayUpdateModel model)
        {

            await _countryRepo.UpdateHoliDay(model);

            return Ok("Holy Day Updated");
        }
    }
}
