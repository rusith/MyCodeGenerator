
using DatabaseSchemaReader.DataSchema;

namespace MyCodeGenerator.Models
{
    public class DBTable
    {
        public string ClassName
        {
            get { return Table.Name + "BO"; }
        }

        public DBTable(DatabaseTable table)
        {
            Table = table;
        }

        public DatabaseTable Table { get; set; }
    }
}
