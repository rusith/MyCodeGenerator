using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;

namespace MyCodeGenerator.Models
{
    public class Sp
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public DatabaseStoredProcedure StoredProcedure { get; set; }
    }
}
