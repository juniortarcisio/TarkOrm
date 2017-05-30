using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Attributes;

namespace TarkOrm.Tests
{
    
    [Table("City")]
    public class City
    {
        [Key]
        public int CityID { get; set; }

        public string Name { get; set; }

        public int? ProvinceID { get; set; }
    }
}
