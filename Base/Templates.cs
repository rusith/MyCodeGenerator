using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyCodeGenerator.Base
{
    public class Templates
    {
        private static Templates _object;
        private Dictionary<string, Template>  _templates;
             
        private Templates()
        {
            ReadTemplate();   
        }

        public static Templates All => _object ?? (_object = new Templates());

        private void ReadTemplate()
        {
            const string templateFile = "\\Templates\\template";
            if(!File.Exists(templateFile))
                return;
            var lines = File.ReadLines(templateFile).ToList();
            if(lines.Count<1)
                return;

            _templates = new Dictionary<string, Template>();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                string currentTemplateName;
                if (!line.StartsWith("##") || line.Length <= 2 ||
                    string.IsNullOrWhiteSpace(currentTemplateName = line.Substring(2, line.Length - 1))) continue;

                var currentTemplateBuilder = new StringBuilder();
                while (line == "###" || i>=(lines.Count-1))
                {
                    currentTemplateBuilder.AppendLine(line);
                    i++;
                }
                _templates.Add(currentTemplateName,new Template(currentTemplateBuilder));
            }
        }

        /// <summary>
        /// Get the template with the given name 
        /// </summary>
        /// <param name="name">name of the template </param>
        /// <returns></returns>
        public Template this[string name] => _templates[name];
    }
}
