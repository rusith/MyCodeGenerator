﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Models;

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

        public static string ReadTemplate(string templateName)
        {
            var templateFile = new FileInfo(Environment.CurrentDirectory+@"\Templates\" + templateName + ".template");
            var content = File.ReadAllText(templateFile.FullName);
            return content.Replace("$projectName$",Settings.ProjectName)
                .Replace("$projectNs$",Settings.ProjectNamespace);
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
            return builder.Replace("$dataType$", column.DataType.NetDataTypeCSharpName)
                .Replace("$name$", Converters.Convert.PropertyNameToFieldName(column.Name)).ToString();
        }

        public static string GenerateReferenceList(string refTableName,string currentTableaName)
        {
            if (_referenceListTemplate == null)
                _referenceListTemplate = ReadTemplate("referenceList");
            var builder = new StringBuilder(_referenceListTemplate);
            return builder.Replace("$refTable$", refTableName)
                .Replace("$tableName$", currentTableaName).ToString();
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
                    .Replace("$freignKeyProperty$", column.Name).ToString();
            }
            else
            {
                if (_propertyTemplate == null)
                    _propertyTemplate = ReadTemplate("property");
                var builder = new StringBuilder(_propertyTemplate);
                return builder.Replace("$dataType$", column.DataType.NetDataTypeCSharpName)
                    .Replace("$name$", column.Name)
                    .Replace("$fieldName$", Converters.Convert.PropertyNameToFieldName(column.Name)).ToString();
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
            foreach (var column in table.Columns)
            {
                fields.Add(GenerateField(column));
                properties.Add(GenerateProperty(column));
                if (!column.IsForeignKey)
                    continue;
                fields.Add(GenerateField(column,true));
                properties.Add(GenerateProperty(column, true));
            }

            var fieldsString = fields.Aggregate((c, n) => c + n);
            var propertiesString = properties.Aggregate((c, n) => c + n);

            var builder = new StringBuilder(_boTemplate);
            return builder.Replace("$tableName$", table.Name)
                .Replace("$fields$", fieldsString)
                .Replace("$properties$", propertiesString).ToString();
        }



        public static string GenerateRepository(DatabaseTable table)
        {

            if (_repositoryTemplate == null)
                _repositoryTemplate = ReadTemplate("RepositoryImpl");
            var builder = new StringBuilder(_repositoryTemplate);
            return builder.Replace("$tableName$", table.Name).ToString();
        }

        public static string GenerateRepositoryCore(DatabaseTable table)
        {
            if (_repsitoryCoreTemplate == null)
                _repsitoryCoreTemplate = ReadTemplate("RepositoryCore");
            var builder = new StringBuilder(_repsitoryCoreTemplate);
            return builder.Replace("$tableName$", table.Name).ToString();
        }
    }
}
