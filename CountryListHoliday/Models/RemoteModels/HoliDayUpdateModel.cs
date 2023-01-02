using System;

namespace CountryListHoliday.Models.RemoteModels
{
    public class HoliDayUpdateModel
    {
        public string HolyDayID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string HoliDayName { get; set; }
    }
}
