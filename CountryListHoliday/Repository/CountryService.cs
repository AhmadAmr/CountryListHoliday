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

               var CountryList = result.Select(x => new Country
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = x.Name.Common,
                    Code = x.Cca2
                }).ToList();

                _context.AddRange(CountryList);
                await _context.SaveChangesAsync();

                return result;
            }

            else
            {
                throw new Exception("Bad Request");
            }
        }

        public Task GetAllHolidaiesASync()
        {
            throw new System.NotImplementedException();
        }
    }
}
