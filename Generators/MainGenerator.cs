using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;
using MyCodeGenerator.Writers;

namespace MyCodeGenerator.Generators
{
    public class MainGenerator
    {
        public static Bo GenerateBo(DatabaseTable table)
        {
            return new Bo { Content = TemplateGenarator.GenerateBo(table), Name = table.Name };
        }

        public static Repository GenerateRepository(DatabaseTable table)
        {
            var repo = new Repository();
            var implContent = TemplateGenarator.GenerateRepository(table);
            var coreContent = TemplateGenarator.GenerateRepositoryCore(table);
            repo.Name = table.Name;
            repo.ImpleContent = implContent;
            repo.CoreContent = coreContent;
            return repo;
        }

        public static View GenerateView(DatabaseView view)
        {
            return new View { Name = view.Name, Content = TemplateGenarator.GenerateView(view) };
        }

        public static Sp GenerateSp(DatabaseStoredProcedure procedure)
        {
            return new Sp { Name = procedure.Name, Content = TemplateGenarator.GenerateStoredProcedure(procedure), StoredProcedure = procedure };
        }

        public static void GenerateReferenceLists(DatabaseSchema schema, List<Bo> bos)
        {
            foreach (var bo in bos)
            {
                var table = schema.Tables.FirstOrDefault(t => t.Name == bo.Name);
                var referencedTables = schema.Tables.Where(t => t.Columns.Count(c => table != null && (c.IsForeignKey && c.ForeignKeyTableName == table.Name)) > 0);
                var builder = new StringBuilder();
                foreach (var rt in referencedTables)
                {
                    var fk = rt.ForeignKeys.FirstOrDefault(f => f.ReferencedTable(schema) == table);
                    if(fk == null)
                        return;
                    
                    builder.Append(TemplateGenarator.GenerateReferenceList(rt.Name, bo.Name, fk.ReferencedColumns(schema).First()));
                }
                    
                bo.Content = bo.Content.Replace("$referenceLists$", builder.ToString());
            }
        }

        private static List<Bo> GenerateTables(DatabaseSchema schema)
        {
            return schema.Tables.Select(GenerateBo).ToList();
        }

        private static List<Repository> GeneRepositories(DatabaseSchema schema)
        {
            return schema.Tables.Select(GenerateRepository).ToList();
        }

        private static List<View> GenerateViews(DatabaseSchema schema)
        {
            return schema.Views.Select(GenerateView).ToList();
        }

        private static List<Sp> GenerateSps(DatabaseSchema schema)
        {
            return schema.StoredProcedures.Select(GenerateSp).ToList();
        }

        public static void Generate(DatabaseSchema schema)
        {
            var repos = GeneRepositories(schema);
            var bos = GenerateTables(schema);
            var views = GenerateViews(schema);
            var sps = GenerateSps(schema);
            GenerateReferenceLists(schema,bos);
            Writer.WriteBase(repos,views,sps);
            Writer.WriteBos(bos);
            Writer.WriteRepositories(repos);
            Writer.WriteViews(views);
            Writer.WriteStoredProcedures(sps);
        }
    }
}
