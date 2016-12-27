using System.Collections.Generic;

namespace MyCodeGenerator.Extensions
{
    public class Templates
    {
        public const string FieldTemplate = "private $dataType$ _$name$;";

        public const string PropertyTemplate = "public $dataType$ $name$ { get { return $fieldName$; } set { if($fieldName$ == value) {return;}  $fieldName$ = value; if(SouldNotifyPropertyChanges){ NotifyPropertyChanged(\"$name$\"); }}}";

        public static readonly HashSet<string> NullableTypes = new HashSet<string>
        {
            "int",
            "short",
            "long",
            "double",
            "decimal",
            "float",
            "bool",
            "DateTime"
        };


    }
}
