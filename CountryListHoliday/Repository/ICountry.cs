using CountryListHoliday.Models;
using CountryListHoliday.Models.RemoteModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryListHoliday.Repository
{
    public interface ICountry
    {
        public Task<List<CountryResponse>> GetAllCountriesAsync();
        public Task GetAllHolidaiesASync();
        public Task<List<Country>> ListAllCountriesAsync(int pageIndex , int PageSize);

        public Task<List<Holiday>> GetHoliDays(string name);

        public Task<bool> AddHoliyDay(HoliDaySubmitModel model);

        public Task<bool> RemoveHoliyDay(string holiDayID);

        public Task<bool> UpdateHoliDay(HoliDayUpdateModel model);
    }
}
