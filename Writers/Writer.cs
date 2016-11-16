
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyCodeGenerator.Generators;
using MyCodeGenerator.Models;

namespace MyCodeGenerator.Writers
{
    public class Writer
    {
        private static void WriteFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static void WriteFile(string path, string content)
        {
            if(!File.Exists(path))
                File.Create(path).Close();
            File.WriteAllText(path,content);
        }

        public static void WriteBase(List<Repository> repositories,List<View> views,List<Sp> sps )
        {
            var basedir = Settings.RootDirectory + "\\Base";
            var core = basedir + "\\Core";
            var impl = basedir + "\\Implementation";
            var idbcontext = core + "\\IDbContext.cs";
            var iunitofWork = core + "\\IUnitOfWork.cs";
            var context = impl + "\\" + Settings.ProjectName + "Context.cs";
            var unitofWork = impl + "\\UnitOfWork.cs";
            var boCollection = impl + "\\BoCollection.cs";

            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(core);
            WriteFolderIfNotExists(impl);

            var repositoryStringCore = new StringBuilder();
            var repositoryString = new StringBuilder();
            var repositoryFieldString = new StringBuilder();

            foreach (var repo in repositories)
            {
                repositoryStringCore.AppendFormat("\n\t\t{0}Repository {0}Repository {{ get; }}", repo.Name);
                repositoryFieldString.AppendFormat("\n\t\tprivate {0}Repository {1};",repo.Name,Converters.Convert.PropertyNameToFieldName(repo.Name)+"Repository");
                repositoryString.AppendFormat("\n\t\tpublic {1}Repository {1}Repository => {0} ?? ({0} = new {1}Repository(_context));", Converters.Convert.PropertyNameToFieldName(repo.Name) + "Repository",repo.Name);
            }

            var viewString = new StringBuilder();
            foreach (var view in views)
                viewString.AppendFormat("\n\t\tpublic List<{0}Bo> {0}(object where=null){{return _context.QueryView<{0}Bo>(\"{0}\",where);}}", view.Name);

            var spString = new StringBuilder();
            foreach (var sp in sps)
            {
                var parameters = sp.StoredProcedure.Arguments;
                var parameterString = parameters.Aggregate("", (current, param) => current + string.Format("{0} {1}", param.DataType.NetDataTypeCSharpName, param.Name)+ ",");
                var parameterPassString = parameters.Aggregate("", (current, param) => current + string.Format("'\"+{0}+\"'", param.Name) + ",");
                spString.AppendFormat("\n\t\tpublic List<{0}Bo> {0}({1}){{ return _context.Query<{0}Bo>(\"EXEC {0} {2}\");}}",sp.StoredProcedure.Name,parameterString.TrimEnd(','),parameterPassString.TrimEnd(','));
            }

            WriteFile(iunitofWork, TemplateGenarator.ReadTemplate("IUnitOfWork").Replace("$repositories$", repositoryStringCore.ToString()));
            WriteFile(idbcontext, TemplateGenarator.ReadTemplate("IDbContext"));
            WriteFile(context,TemplateGenarator.ReadTemplate("Context"));
            WriteFile(unitofWork,TemplateGenarator.ReadTemplate("UnitOfWork")
                .Replace("$repositories$", repositoryString.ToString())
                .Replace("$repoFields$", repositoryFieldString.ToString())
                .Replace("$views$",viewString.ToString())
                .Replace("$storedProcedures$",spString.ToString()));
            WriteFile(boCollection,TemplateGenarator.ReadTemplate("BoCollection"));
        }

        private static void WriteBo(Bo bo,string basePath)
        {
            WriteFile(basePath+"\\"+bo.Name+"Bo.cs",bo.Content);
        }

        private static void WriteView(View view,string baseDir)
        {
            WriteFile(baseDir+"\\"+view.Name+"Bo.cs",view.Content);
        }

        public static void WriteBos(List<Bo> bos)
        {
            var directory = Settings.RootDirectory;
            var objectsDirectory = new DirectoryInfo(directory.FullName + "\\Objects");
            var coreDirectory = new DirectoryInfo(objectsDirectory.FullName + "\\Core");

            WriteFolderIfNotExists(objectsDirectory.FullName);
            WriteFolderIfNotExists(coreDirectory.FullName);

            var implementationDirectory = new DirectoryInfo(objectsDirectory + "\\Implementation");
            WriteFolderIfNotExists(implementationDirectory.FullName);

            var entity = new FileInfo(implementationDirectory.FullName + "\\Entity.cs");
            WriteFile(entity.FullName, TemplateGenarator.ReadTemplate("Entity"));

            foreach (var bo in bos)
                WriteBo(bo, implementationDirectory.FullName);
        }

        private static void WriteRepository(Repository repo,string basePath)
        {
            WriteFile(basePath + "\\Core\\I" + repo.Name + "Repository.cs", repo.CoreContent);
            WriteFile(basePath+"\\Implementation\\"+repo.Name+"Repository.cs",repo.ImpleContent);
        }

        public static void WriteRepositories(List<Repository> repos)
        {
            var basedir = Settings.RootDirectory + "\\Repositories";
            var core = basedir + "\\Core";
            var impl = basedir + "\\Implementation";
            var repository = impl + "\\Repository.cs";

            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(core);
            WriteFolderIfNotExists(impl);

            WriteFile(repository,TemplateGenarator.ReadTemplate("Repository"));

            foreach (var repo in repos)
                WriteRepository(repo, basedir);
        }

        public static void WriteViews(List<View> views)
        {
            var basedir = Settings.RootDirectory + "\\Objects";
            var viewdir = basedir + "\\Views";

            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(viewdir);

            foreach (var view in views)
                WriteView(view, viewdir);

           
        }

        private static void WriteSp(Sp sp,string baseDir)
        {
            WriteFile(baseDir + "\\" + sp.Name + "Bo.cs", sp.Content);
        }

        public static void WriteStoredProcedures(List<Sp> sps)
        {
            var basedir = Settings.RootDirectory + "\\Objects";
            var spdir = basedir + "\\SPs";

            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(spdir);

            foreach (var sp in sps)
                WriteSp(sp, spdir);
        }
    }
}
