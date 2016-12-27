using DatabaseSchemaReader.DataSchema;

namespace MyCodeGenerator.Extensions
{
    public static class TableExtensions
    {
        public static string GetClassName(this DatabaseTable table)
        {
            return table.Name + "BO";
        }
    }
}
