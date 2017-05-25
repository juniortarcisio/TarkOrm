using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Attributes
{
    public class DatabaseAttribute : Attribute
    {
        public string Name { get; set; }

        public string Server { get; set; }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">database name</param>
        /// <param name="server"></param>
        public DatabaseAttribute(string name, string server)
        {
            Name = name;
            Server = server;
        }
    }
}
