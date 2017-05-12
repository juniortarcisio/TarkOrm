using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.NET.Attributes
{
    public class IdentityAttribute : Attribute
    {
        public IdentityAttribute()
        {
        }

        public TarkQueryBuilder TarkQueryBuilder
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
    }
}
