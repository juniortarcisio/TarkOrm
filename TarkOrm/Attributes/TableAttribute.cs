using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; }

        public TableAttribute(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public TableAttribute(string name)
        {
            Name = name;
            Schema = "dbo";
        }
    }
}
