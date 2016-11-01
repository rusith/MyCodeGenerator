
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class BoGenerator
    {
        public static Bo Generate(DatabaseTable table)
        {
            return new Bo() {Content = TemplateGenarator.GenerateBo(table),Name = table.Name};
        }

        public static void GenerateReferenceLists(DatabaseSchema schema, List<Bo> bos)
        {
            foreach (var bo in bos)
            {
                var table = schema.Tables.FirstOrDefault(t => t.Name == bo.Name);
                var referencedTables = schema.Tables.Where(t => t.Columns.Count(c => c.IsForeignKey && c.ForeignKeyTableName == table.Name) > 0);
                var builder = new StringBuilder();
                foreach (var rt in referencedTables)
                {
                    builder.Append(TemplateGenarator.GenerateReferenceList(rt.Name, bo.Name));
                }
                bo.Content = bo.Content.Replace("$referenceLists$", builder.ToString());
            }
        }
    }
}
