using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Attributes
{
    public class IdentityAttribute : Attribute
    {
        public IdentityAttribute()
        {
        }

        public TarkQueryBuilder QueryBuilder
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
