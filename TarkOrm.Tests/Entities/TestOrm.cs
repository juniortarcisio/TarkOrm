using System;
using TarkOrm.Attributes;

namespace TarkOrm.Tests
{
    public class TestOrm
    {
        [Key]
        [Readonly]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? CreationDate { get; set; }

        public char Classification { get; set; }
    }


    //[Table("TestOrm")]
    public class TestOrmTestMapping
    {
        [Key]
        [Readonly]
        public int Id { get; set; }

        public string description { get; set; }

        public DateTime? CreationDate { get; set; }

        public char classx { get; set; }
    }
}
