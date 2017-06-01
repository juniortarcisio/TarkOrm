using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TarkOrm.Attributes;

namespace TarkOrm.Mapping
{

    public class TarkColumnMapping
    {
        public PropertyInfo Property { get; set; }

        public HashSet<Attribute> Attributes { get; set; }

        public TarkColumnMapping() { }

        public TarkColumnMapping(PropertyInfo property)
        {
            Property = property;
            Attributes = new HashSet<Attribute>();

            foreach (Attribute attribute in Property.GetCustomAttributes())
            {
                Attributes.Add(attribute);
            }
        }

        public TarkColumnMapping(PropertyInfo property, Attribute attribute)
        {
            Property = property;
            Attributes = new HashSet<Attribute>() { attribute };
        }

        public TarkColumnMapping(PropertyInfo property, params Attribute[] attributes)
        {
            Property = property;
            Attributes = new HashSet<Attribute>(attributes);
        }

        public bool IsKeyColumn()
        {
            return Attributes.Any(x => x is KeyAttribute);
        }

        public bool IsReadOnlyColumn()
        {
            return Attributes.Any(x => x is ReadOnlyAttribute);
        }
    }
}
