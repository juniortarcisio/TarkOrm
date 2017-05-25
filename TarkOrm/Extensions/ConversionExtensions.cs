using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkOrm.Extensions
{
    public static class ConversionExtensions
    {
        public static TarkTransformer TarkTransformer
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
        /// Try to converts the value into another type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="convert"></param>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns>The object value converted</returns>
        public static object To(this object value, Type conversionType)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// Try to converts the value into another type
        /// http://stackoverflow.com/questions/793714/how-can-i-fix-this-up-to-do-generic-conversion-to-nullablet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="convert"></param>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns>The object value converted</returns>
        public static object To<T>(this object value)
        {
            Type typeT = typeof(T);

            if (typeT.IsGenericType && typeT.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(typeT);
                return Convert.ChangeType(value, underlyingType);
            }
            else
            {
                return Convert.ChangeType(value, typeT);
            }
        }
    }
}
