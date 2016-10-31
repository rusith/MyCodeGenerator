
using System;
using System.Linq;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class BoGenerator
    {
        public static Bo Generate(DatabaseTable table)
        {
            var fields = table.Columns.Select(column => "\tprivate " + column.DataType.NetDataTypeCSharpName + " " + " _" + Char.ToLowerInvariant(column.Name[0]) + column.Name.Substring(1) + ";").ToList();
            var properties = table.Columns.Select(column => string.Format("\tpublic {0}{1} {2} {3}", column.DataType.NetDataTypeCSharpName, column.Nullable ? "?" : "", column.Name, @"{get {return _" + Char.ToLowerInvariant(column.Name[0]) + column.Name.Substring(1) + "; }" + " set{ _" + Char.ToLowerInvariant(column.Name[0]) + column.Name.Substring(1) + " = value; " + (column.Name != "ID" ? "NotifyPropertyChanged(\"" + column.Name + "\");" : "") + "} " + "}")).ToList();
            var classTemplate = string.Format(
                "using Dapper;\n" +
                "using System.ComponentModel;\n\n\n" +
                "[Table(\"{0}\")]\n" +
                "public class {0} : Entity\n" +
                "{{\n\n" +
                "#region Fields\n\n" +
                "{1}\n" +
                "#endregion\n\n" +
                "#region Properties\n\n" +
                "{2}\n" +
                "#endregion\n\n" +
                "}}",table.Name,fields.Aggregate((c,n)=> c+n+"\n"), properties.Aggregate((c, n) => c + n + "\n")
                );
            return new Bo() {Content = classTemplate,Name = table.Name};
        }
    }
}
