using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarkOrm.Attributes;

namespace TarkOrm.Tests
{
    public class TestOrm
    {
        [Key]
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? CreationDate { get; set; }

        public char Classification { get; set; }
    }
}
