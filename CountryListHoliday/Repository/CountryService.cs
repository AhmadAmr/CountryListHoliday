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

                await RemoveCurrentList();
                await AddCurrentList(result);

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


        private async Task GetHoliday(Country currentCountry)
        {
            string url = $"calendar/v3/calendars/en.EG%23holiday%40group.v.calendar.google.com/events?key={_env.APiKey}";

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_env.HoliDayBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<HolidayResponse>(response.Content.ReadAsStringAsync().Result);

                currentCountry.Holidays = result.Items.Select(x=> new Holiday
                {
                    Start = x.Start.Date,
                    End = x.End.Date,
                    ID = Guid.NewGuid().ToString(),
                    HolidayName = x.Summary
                }).ToList();
                _context.Countries.Update(currentCountry);
                await _context.SaveChangesAsync();

                
            }

            else
            {
                throw new Exception("Bad Request");
            }
        }

        private async Task RemoveCurrentList()
        {
            var currentCountries = await _context.Countries.ToListAsync();
            _context.Countries.RemoveRange(currentCountries);
            await _context.SaveChangesAsync();
        }

        private async Task AddCurrentList(List<CountryResponse> result)
        {
            var CountryList = result.Select(x => new Country
            {
                ID = Guid.NewGuid().ToString(),
                Name = x.Name.Common,
                Code = x.Cca2
            }).ToList();

            _context.AddRange(CountryList);
            await _context.SaveChangesAsync();
        }
    }
}
