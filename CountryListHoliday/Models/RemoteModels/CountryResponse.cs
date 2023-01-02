using Newtonsoft.Json;
using System.Xml.Linq;

namespace CountryListHoliday.Models.RemoteModels
{
    public class CountryResponse
    {
        [JsonProperty("cca2")]
        public string Cca2 { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("common")]
        public string Common { get; set; }

        [JsonProperty("official")]
        public string Official { get; set; }

    }
}

