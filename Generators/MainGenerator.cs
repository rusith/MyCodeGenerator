using System.Collections.Generic;
using System.Linq;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;
using MyCodeGenerator.Writers;

namespace MyCodeGenerator.Generators
{
    public class MainGenerator
    {
        private static List<Bo> GenerateTables(DatabaseSchema schema)
        {
            var tables = new List<Bo>();
            foreach (var table in schema.Tables)
            {
                tables.Add(BoGenerator.Generate(table));
            }
            return tables;
        }

        private static List<Repository> GeneRepositories(DatabaseSchema schema)
        {
            return schema.Tables.Select(RepositoryGenerator.Generate).ToList();
        } 

        public static void Generate(DatabaseSchema schema)
        {
            var repos = GeneRepositories(schema);
            var bos = GenerateTables(schema);
            BoGenerator.GenerateReferenceLists(schema,bos);
            Writer.WriteBase(repos);
            Writer.WriteBos(bos);
            Writer.WriteBos(repos);

        }
    }
}
