using System;

namespace CountryListHoliday.Models.RemoteModels
{
    public class HoliDaySubmitModel
    {
        public string CountryID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string HoliDayName { get; set; }
    }
}
