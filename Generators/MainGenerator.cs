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
            return schema.Tables.Select(BoGenerator.Generate).ToList();
        }

        private static List<Repository> GeneRepositories(DatabaseSchema schema)
        {
            return schema.Tables.Select(RepositoryGenerator.Generate).ToList();
        }

        private static List<View> GenerateViews(DatabaseSchema schema)
        {
            return schema.Views.Select(ViewGenerator.Generate).ToList();
        }

        private static List<Sp> GenerateSps(DatabaseSchema schema)
        {
            return schema.StoredProcedures.Select(SpGenerator.Generate).ToList();
        } 

        public static void Generate(DatabaseSchema schema)
        {
            var repos = GeneRepositories(schema);
            var bos = GenerateTables(schema);
            var views = GenerateViews(schema);
            var sps = GenerateSps(schema);
            BoGenerator.GenerateReferenceLists(schema,bos);
            Writer.WriteBase(repos,views,sps);
            Writer.WriteBos(bos);
            Writer.WriteRepositories(repos);
            Writer.WriteViews(views);
            Writer.WriteStoredProcedures(sps);
        }
    }
}
