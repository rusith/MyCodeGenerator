using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class SpGenerator
    {
        public static Sp Generate(DatabaseStoredProcedure procedure)
        {
            return new Sp { Name = procedure.Name, Content = TemplateGenarator.GenerateStoredProcedure(procedure),StoredProcedure = procedure};
        }
    }
}
