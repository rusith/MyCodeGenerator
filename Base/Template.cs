using System.Text;

namespace MyCodeGenerator.Base
{
    public class Template
    {
        private StringBuilder _builder;

        public Template(StringBuilder str)
        {
            _builder = str;
        }

        /// <summary>
        /// Set a variable value of the template
        /// </summary>
        /// <param name="name">value name to set</param>
        /// <param name="replacement">value to set</param>
        /// <returns></returns>
        public Template Set(string name, string replacement)
        {
            _builder.Replace("$" + name + "$", replacement);
            return this;
        }

        public string Build()
        {
            return _builder.ToString();
        }
    }
}
