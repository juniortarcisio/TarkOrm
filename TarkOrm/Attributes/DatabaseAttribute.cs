using System;

namespace TarkOrm.Attributes
{
    public class DatabaseAttribute : Attribute
    {
        public string Name { get; set; }

        public string Server { get; set; }

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
