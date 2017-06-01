using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Mapping
{
    public class DuplicatedMappingException : Exception
    {
        public DuplicatedMappingException(string columnName, string typeName) : base(SetErrorMessage(columnName, typeName))
        {
        }

        private static string SetErrorMessage(string columnName, string typeName)
        {
            return $"Duplicated mapping on the column {columnName} and type {typeName}";
        }
    }
}
