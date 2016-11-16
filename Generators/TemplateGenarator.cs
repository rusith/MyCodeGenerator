using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DatabaseSchemaReader.DataSchema;

namespace MyCodeGenerator.Generators
{
    public class TemplateGenarator
    {
        private static string  _fieldTemplate;
        private static string _propertyTemplate;
        private static string _referencePropertyTemplate;
        private static string _boTemplate;
        private static string _repositoryTemplate;
        private static string _repsitoryCoreTemplate;
        private static string _referenceListTemplate;
        private static string _viewTemplate;
        private static string _autoPropertyTemplate;
        private static string _procedureTemplate;

        private static readonly HashSet<string> NullableTypes = new HashSet<string>
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

        public static string ReadTemplate(string templateName)
        {
            var templateFile = new FileInfo(Environment.CurrentDirectory+@"\Templates\" + templateName + ".template");
            var content = File.ReadAllText(templateFile.FullName);
            return content.Replace("$projectName$",Settings.ProjectName)
                .Replace("$projectNs$",Settings.ProjectNamespace)
                .Replace("$topComment$", "/*This code is a generated one , Change the source code of the generator if you want some change in this code"+
                                "\nYou can find the source code of the code generator from here -> https://github.com/rusith/MyCodeGenerator*/");
        }

        public static string GenerateField(DatabaseColumn column,bool normal=false)
        {
            if (_fieldTemplate == null)
                _fieldTemplate = ReadTemplate("field");
            var builder = new StringBuilder(_fieldTemplate);
            if (column.IsForeignKey && !normal)
            {
                return builder.Replace("$dataType$", column.ForeignKeyTableName + "Bo")
                    .Replace("$name$", Converters.Convert.PropertyNameToFieldName("Obj" + column.Name)).ToString();
            }
            return builder.Replace("$dataType$", column.DataType.NetDataTypeCSharpName+ (column.Nullable && NullableTypes.Contains(column.DataType.NetDataTypeCSharpName) ? "?" : ""))
                .Replace("$name$", Converters.Convert.PropertyNameToFieldName(column.Name)).ToString();
        }

        public static string GenerateReferenceList(string refTableName,string currentTableaName,string currentTablePrimaryKey)
        {
            if (_referenceListTemplate == null)
                _referenceListTemplate = ReadTemplate("referenceList");
            var builder = new StringBuilder(_referenceListTemplate);
            return builder.Replace("$refTable$", refTableName)
                .Replace("$tableName$", currentTableaName)
                .Replace("$primaryKeyName$",currentTablePrimaryKey).ToString();
        }

        public static string GenerateAutoProperty(DatabaseColumn column)
        {
            if (_autoPropertyTemplate == null)
                _autoPropertyTemplate = ReadTemplate("autoProperty");
            var builder = new StringBuilder(_autoPropertyTemplate);
            return builder.Replace("$dataType$", column.DataType.NetDataTypeCSharpName + (column.Nullable && NullableTypes.Contains(column.DataType.NetDataTypeCSharpName) ? "?" : ""))
                .Replace("$name$", column.Name).ToString();
        }

        public static string GenerateAutoPropertySp(DatabaseColumn column)
        {
            if (_autoPropertyTemplate == null)
                _autoPropertyTemplate = ReadTemplate("autoProperty");
            var builder = new StringBuilder(_autoPropertyTemplate);
            var dataType = new DataType(column.DbDataType,column.DbDataType);
            return builder.Replace("$dataType$", dataType.NetDataTypeCSharpName + (column.Nullable && NullableTypes.Contains(dataType.NetDataTypeCSharpName) ? "?" : ""))
                .Replace("$name$", column.Name).ToString();
        }

        public static string GenerateProperty(DatabaseColumn column,bool normal=false)
        {
            if (column.IsForeignKey && !normal)
            {
                if (_referencePropertyTemplate == null)
                    _referencePropertyTemplate = ReadTemplate("referenceProperty");
                var builder = new StringBuilder(_referencePropertyTemplate);
                return builder.Replace("$dataType$", column.ForeignKeyTableName + "Bo")
                    .Replace("$name$", "Obj" + column.Name)
                    .Replace("$fieldName$", Converters.Convert.PropertyNameToFieldName("Obj" + column.Name))
                    .Replace("$foreignTableName$", column.ForeignKeyTableName)
                    .Replace("$freignKeyProperty$", column.Name).ToString()
                    .Replace("$keyPropertyAccessor$", string.Format(column.Nullable ? "{0}.GetValueOrDefault()" : "{0}", column.Name))
                    .Replace("$foreignTablePropertyName$",column.ForeignKeyTable.PrimaryKeyColumn.Name);
            }
            else
            {
                if (_propertyTemplate == null)
                    _propertyTemplate = ReadTemplate("property");
                var builder = new StringBuilder(_propertyTemplate);
                return builder.Replace("$dataType$", column.DataType.NetDataTypeCSharpName + (column.Nullable && NullableTypes.Contains(column.DataType.NetDataTypeCSharpName) ? "?" : ""))
                    .Replace("$name$", column.Name)
                    .Replace("$fieldName$", Converters.Convert.PropertyNameToFieldName(column.Name))
                    .Replace("$primary$", column.IsPrimaryKey?"PrimaryKey = value;":"").ToString();
            }
        }

        public static string GenerateBo(DatabaseTable table)
        {
            if (table == null)
                return "";

            if (_boTemplate == null)
                _boTemplate = ReadTemplate("bo");

            var fields = new List<string>();
            var properties = new List<string>();
            var copyValues = new List<string>();
            foreach (var column in table.Columns)
            {


                fields.Add(GenerateField(column));
                properties.Add(GenerateProperty(column));
                if (column.IsForeignKey)
                {
                    fields.Add(GenerateField(column, true));
                    properties.Add(GenerateProperty(column, true));
                }

                

                string fieldName;
                if (column.IsForeignKey)
                {
                    fieldName = Converters.Convert.PropertyNameToFieldName("Obj" + column.Name);
                    copyValues.Add(string.Format("\n\t\t\t{0} = entity.{0};", Converters.Convert.PropertyNameToFieldName(column.Name)));
                }
                else fieldName = Converters.Convert.PropertyNameToFieldName(column.Name);
                copyValues.Add(string.Format("\n\t\t\t{0} = entity.{0};", fieldName));

            }

            var fieldsString = fields.Aggregate((c, n) => c + n);
            var propertiesString = properties.Aggregate((c, n) => c + n);
            var columnString = new StringBuilder();
            foreach (var col in table.Columns)
            {
                columnString.AppendFormat("\n\t\t\t\t{{\"{0}\", {0}}},", col.Name);
            }

            var primeKey = "";
            if (table.PrimaryKey != null)
                primeKey = table.PrimaryKeyColumn.Name;

            var builder = new StringBuilder(_boTemplate);
            return builder.Replace("$tableName$", table.Name)
                .Replace("$fields$", fieldsString)
                .Replace("$properties$", propertiesString)
                .Replace("$columnValueMappings$", columnString.ToString().TrimEnd(','))
                .Replace("$copyProperties$", copyValues.Aggregate((c, n) => c + n))
                .Replace("$primaryKeyName$",primeKey).ToString();
        }



        public static string GenerateRepository(DatabaseTable table)
        {

            if (_repositoryTemplate == null)
                _repositoryTemplate = ReadTemplate("RepositoryImpl");
            var builder = new StringBuilder(_repositoryTemplate);
            var primeKey = "";
            if (table.PrimaryKey != null)
                primeKey = table.PrimaryKeyColumn.Name;

            var t= builder.Replace("$tableName$",table.Name).Replace("$primaryKeyName$",primeKey).Replace("$tableFullName$", "["+table.SchemaOwner+"].["+table.Name+"]").ToString();
            if (primeKey == "")
            {
                t = t.Replace("$overrideMethods$",
                    "public override " + table.Name + "Bo Get(int id){ return null; }" +
                    "\n\t\tpublic override void Delete(int id) { }");
            }
            else t = t.Replace("$overrideMethods$", "");
            return t;
        }

        public static string GenerateRepositoryCore(DatabaseTable table)
        {
            if (_repsitoryCoreTemplate == null)
                _repsitoryCoreTemplate = ReadTemplate("RepositoryCore");
            var builder = new StringBuilder(_repsitoryCoreTemplate);
            return builder.Replace("$tableName$", table.Name).ToString();
        }

        public static string GenerateView(DatabaseView view)
        {
            if (_viewTemplate == null)
                _viewTemplate = ReadTemplate("view");
            var builder = new StringBuilder(_viewTemplate);
            var properties = view.Columns.Select(GenerateAutoProperty).ToList();
            var propertyString = properties.Aggregate((c, n) => c + n);
            return builder.Replace("$viewName$", view.Name)
                .Replace("$properties$", propertyString).ToString();
        }

        public static string GenerateStoredProcedure(DatabaseStoredProcedure proc)
        {
            if (_procedureTemplate == null)
                _procedureTemplate = ReadTemplate("storedProcedure");
            var builder = new StringBuilder(_procedureTemplate);
            var sprocRunner = new DatabaseSchemaReader.Procedures.ResultSetReader(proc.DatabaseSchema);
            sprocRunner.ExecuteProcedure(proc);
            var resultsets = proc.ResultSets;
            var propertyString = "";
            if (resultsets.Count <= 0)
                return builder.Replace("$name$", proc.Name)
                    .Replace("$properties$", propertyString).ToString();
            var properties = proc.ResultSets.First().Columns.Select(GenerateAutoPropertySp).ToList();
            propertyString = properties.Aggregate((c, n) => c + n);
            return builder.Replace("$name$", proc.Name)
                .Replace("$properties$", propertyString).ToString();
        }

        public static void Clear()
        {
            _fieldTemplate = null;
            _propertyTemplate = null;
            _referencePropertyTemplate = null;
            _boTemplate = null;
            _repositoryTemplate = null;
            _repsitoryCoreTemplate = null;
            _referenceListTemplate = null;
            _viewTemplate = null;
            _autoPropertyTemplate = null;
        }
    }
}
