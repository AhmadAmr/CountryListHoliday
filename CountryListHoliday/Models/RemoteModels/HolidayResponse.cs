using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace CountryListHoliday.Models.RemoteModels
{
    public class HolidayResponse
    {

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

    }

    public partial class Item
    {

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("start")]
        public End Start { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }

    }
    public partial class End
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
