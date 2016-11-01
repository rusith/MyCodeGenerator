using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class RepositoryGenerator
    {
        public static Repository Generate(DatabaseTable table)
        {
            var repo = new Repository();
            var implContent = TemplateGenarator.GenerateRepository(table);
            var coreContent = TemplateGenarator.GenerateRepositoryCore(table);
            repo.Name = table.Name;
            repo.ImpleContent = implContent;
            repo.CoreContent = coreContent;
            return repo;
        }
    }
}
