using CountryListHoliday.Models;
using CountryListHoliday.Models.RemoteModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CountryListHoliday.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using CountryListHoliday.Extentions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Localization;

namespace CountryListHoliday.Repository
{
    public class CountryService : ICountry
    {
        private readonly CountriesConf _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;

        public CountryService(IOptions<CountriesConf> env, IHttpClientFactory httpClientFactory, ApplicationDbContext context)
        {
            _env = env.Value;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        public async Task<List<CountryResponse>> GetAllCountriesAsync()
        {
            string url = "v3.1/all";

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_env.CoutryBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            var r = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<CountryResponse>>(response.Content.ReadAsStringAsync().Result);

                await AddOrUpdate(result);

                return result;
            }

            else
            {
                throw new Exception("Bad Request");
            }
        }

        public async Task GetAllHolidaiesASync()
        {
           var countries =  await _context.Countries.ToListAsync();

            foreach(var country in countries)
            {
                await GetHoliday(country);
            }

        }

        public  async Task<List<Country>> ListAllCountriesAsync(int pageIndex,int pageSize)
        {
           
           var countries =  _context.Countries.AsQueryable();

            return  await PaginatedList<Country>.CreateAsync(countries, pageIndex, pageSize);
        }

        public async Task<List<Holiday>> GetHoliDays(string name)
        {
           return await _context.Countries.Where(X => X.Name.Contains(name)).Select(x => x.Holidays).FirstOrDefaultAsync();
        }


        public async Task<bool> AddHoliyDay(HoliDaySubmitModel model)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x=> x.ID == model.CountryID);
            if (country != null)
            {
                _context.Holidays.Add(new Holiday
                {
                    Country = country,
                    ID = Guid.NewGuid().ToString(),
                    Start = model.Start,
                    End = model.End,
                    HolidayName = model.HoliDayName
                });
                await _context.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<bool> RemoveHoliyDay(string holiDayID)
        {
            var holiday = await _context.Holidays.FirstOrDefaultAsync(x => x.ID == holiDayID);
            if(holiday != null)
            {
                _context.Holidays.Remove(holiday);
                 await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateHoliDay(HoliDayUpdateModel model)
        {
            var holiday = await _context.Holidays.FirstOrDefaultAsync(x => x.ID == model.HolyDayID);

            if(holiday != null)
            {
                holiday.Start = model.Start;
                holiday.End = model.End;
                holiday.HolidayName = model.HoliDayName;

                _context.Holidays.Update(holiday);
                await _context.SaveChangesAsync();
            }
            return false;
        }



        private async Task GetHoliday(Country currentCountry)
        {
            string url = $"calendar/v3/calendars/en.{currentCountry.Code}%23holiday%40group.v.calendar.google.com/events?key={_env.APiKey}";

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_env.HoliDayBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<HolidayResponse>(response.Content.ReadAsStringAsync().Result);


                var holidays = result.Items.Select(x => new Holiday
                {
                    Start = x.Start.Date,
                    End = x.End.Date,
                    ID = Guid.NewGuid().ToString(),
                    HolidayName = x.Summary,
                    Country = currentCountry
                }).ToList();

                _context.Holidays.AddRange(holidays);
                await _context.SaveChangesAsync();

                
            }

        }

        private async Task AddOrUpdate(List<CountryResponse> result)
        {
            var currentList = await _context.Countries.ToListAsync();

            var newItems = result.Where(x => !currentList.Select(p=> p.Code).Contains(x.Cca2)).ToList();
            var oldItems = currentList.Where(x => result.Select(p => p.Cca2).Contains(x.Code)).ToList();
            foreach(var item in oldItems){ 

                var curr = result.Where(a => a.Cca2 == item.Code).FirstOrDefault();
                item.Name = curr.Name.Common;
               }

            var NewCountryList = newItems.Select(x => new Country
            {
                ID = Guid.NewGuid().ToString(),
                Name = x.Name.Common,
                Code = x.Cca2
            }).ToList();
            


            _context.AddRange(NewCountryList);
            _context.UpdateRange(oldItems);
            await _context.SaveChangesAsync();
        }

      
    }
}
