using CountryListHoliday.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet]
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

    }
}
