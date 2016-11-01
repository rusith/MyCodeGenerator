using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeGenerator.Converters
{
    public class Convert
    {
        public static string PropertyNameToFieldName(string property)
        {
            if (property == null)
                return "";
            if (property.Length < 1)
                return "";
            var firstCharactor = property[0];
            if (property.Length < 2)
                return "_" + ("" + firstCharactor).ToLower();
            return "_" + ("" + firstCharactor).ToLower() + property.Substring(1);
        }
    }
}
