using System;
using System.ComponentModel.DataAnnotations;

namespace CountryListHoliday.Models
{
    public class Holiday
    {
        [Key]
        public string ID { get; set; }
        public string HolidayName { get; set; }
        public DateTime End{ get; set; }
        public DateTime  Start { get; set; }
        public virtual Country Country { get; set; }
    }
}
