using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Tests
{
    public class Country
    {
        [Key]        
        public int CountryID { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public int ContinentID { get; set; }

        [Column("flagb64")]
        public string FlagB64{ get; set; }

        public int CurrencyID { get; set; }
    }

    [Table("Country")]
    public class CountryPartial
    {
        [Key]
        public int CountryID { get; set; }

        public string CountryCode { get; set; }

        public string name { get; set; }
    }
}
