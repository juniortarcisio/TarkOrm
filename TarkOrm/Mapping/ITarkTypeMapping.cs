using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Mapping
{
    public interface ITarkTypeMapping
    {
        Type GetMappingType();

        Dictionary<string, TarkColumnMapping> GetPropertiesMapping();
    }
}
