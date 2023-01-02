using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CountryListHoliday.Models
{
    public class Country
    {
        [Key]
        public string ID { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public List<Holiday> Holidays { get; set; } = new List<Holiday>();
    }
}
