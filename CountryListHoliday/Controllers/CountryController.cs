using CountryListHoliday.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task SyncCountries()
        {
            await _countryRepo.GetAllCountriesAsync();
        }

    }
}
