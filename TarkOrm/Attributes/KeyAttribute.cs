using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Attributes
{
    public class KeyAttribute : Attribute
    {
        public int Order { get; set; }

        public KeyAttribute() { }

        public KeyAttribute(int order)
        {
            Order = order;
        }

    }
}
