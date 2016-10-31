using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class MainGenerator
    {
        private static List<Bo> GenerateTables(DatabaseSchema schema)
        {
            return schema.Tables.Select(BoGenerator.Generate).ToList();
        }

        public static void Generate(DatabaseSchema schema)
        {
            var tables = GenerateTables(schema);
        }
    }
}
