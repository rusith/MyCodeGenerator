using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Generators
{
    public class ViewGenerator
    {
        public static View Generate(DatabaseView view)
        {
            return new View {Name = view.Name,Content = TemplateGenarator.GenerateView(view)};
        }
    }
}
