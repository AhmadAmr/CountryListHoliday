using CountryListHoliday.Models.RemoteModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryListHoliday.Repository
{
    public interface ICountry
    {
        public Task<List<CountryResponse>> GetAllCountriesAsync();

        public Task GetAllHolidaiesASync();
    }
}
