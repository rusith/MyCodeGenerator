using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;

namespace MyCodeGenerator.Extensions
{
    public static class ColumnExtensions
    {
        public static string GetDataType(this DatabaseColumn column)
        {
            var dataType = column.IsForeignKey ?
              column.ForeignKeyTable.GetClassName() :
              column.DataType.NetDataTypeCSharpName;

            if (column.Nullable && !column.IsForeignKey && Templates.NullableTypes.Contains(dataType))
                dataType += "?";

            return dataType;
        }

        public static string GetField(this DatabaseColumn column)
        {
            var dataType = column.GetDataType();

            return new StringBuilder(Templates.FieldTemplate)
                .Replace("$dataType$", dataType)
                .Replace("$name$", (column.Name.First() + "").ToLower() + column.Name.Substring(1, column.Name.Length - 1))
                .ToString();
        }

        public static string GetProperty(this DatabaseColumn column)
        {
            var dataType = column.GetDataType();
            return new StringBuilder(Templates.PropertyTemplate)
                .Replace("$dataType$", dataType)
                .Replace("$name$",column.Name)
                .ToString();
        }
    }
}
