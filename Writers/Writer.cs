
using System.Collections.Generic;
using System.IO;
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

        public static void WriteBase(List<Repository> repositories )
        {
            var basedir = Settings.RootDirectory + "\\Base";
            var core = basedir + "\\Core";
            var impl = basedir + "\\Implementation";
            var idbcontext = core + "\\IDbContext.cs";
            var iunitofWork = core + "\\IUnitOfWork.cs";
            var context = impl + "\\" + Settings.ProjectName + "Context.cs";
            var unitofWork = impl + "\\UnitOfWork.cs";


            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(core);
            WriteFolderIfNotExists(impl);



            var repositoryStringCore = new StringBuilder();
            var repositoryString = new StringBuilder();
            var repoInitializeString = new StringBuilder();
            foreach (var repo in repositories)
            {
                repositoryStringCore.AppendFormat("\n\t\tI{0}Repository {0}Repository {{ get; set; }}", repo.Name);
                repositoryString.AppendFormat("\n\t\tpublic I{0}Repository {0}Repository {{ get; set; }}", repo.Name);
                repoInitializeString.AppendFormat("\n\t\t\t{0}Repository = new {0}Repository(_context);", repo.Name);
            }

            WriteFile(iunitofWork, TemplateGenarator.ReadTemplate("IUnitOfWork").Replace("$repositories$", repositoryStringCore.ToString()));
            WriteFile(idbcontext, TemplateGenarator.ReadTemplate("IDbContext"));
            WriteFile(context,TemplateGenarator.ReadTemplate("Context"));
            WriteFile(unitofWork,TemplateGenarator.ReadTemplate("UnitOfWork").Replace("$repositories$", repositoryString.ToString()).Replace("$repoInit$",repoInitializeString.ToString()));
        }

        private static void WriteBo(Bo bo,string basePath)
        {
            WriteFile(basePath+"\\"+bo.Name+"Bo.cs",bo.Content);
        }

        public static void WriteBos(List<Bo> bos)
        {
            var directory = Settings.RootDirectory;
            var objectsDirectory = new DirectoryInfo(directory.FullName + "\\Objects");
            var coreDirectory = new DirectoryInfo(objectsDirectory.FullName + "\\Core");

            WriteFolderIfNotExists(objectsDirectory.FullName);
            WriteFolderIfNotExists(coreDirectory.FullName);

            var ientity = new FileInfo(coreDirectory + @"\IEntity.cs");
            WriteFile(ientity.FullName, TemplateGenarator.ReadTemplate("IEntity"));

            var implementationDirectory = new DirectoryInfo(objectsDirectory + "\\Implementation");
            WriteFolderIfNotExists(implementationDirectory.FullName);

            var entity = new FileInfo(implementationDirectory.FullName + "\\Entity.cs");
            WriteFile(entity.FullName, TemplateGenarator.ReadTemplate("Entity"));

            foreach (var bo in bos)
            {
                WriteBo(bo,implementationDirectory.FullName);
            }
        }

        private static void WriteRepository(Repository repo,string basePath)
        {
            WriteFile(basePath + "\\Core\\I" + repo.Name + "Repository.cs", repo.CoreContent);
            WriteFile(basePath+"\\Implementation\\"+repo.Name+"Repository.cs",repo.ImpleContent);
        }

        public static void WriteBos(List<Repository> repos)
        {
            var basedir = Settings.RootDirectory + "\\Repositories";
            var core = basedir + "\\Core";
            var impl = basedir + "\\Implementation";
            var irepository = core + "\\IRepository.cs";
            var repository = impl + "\\Repository.cs";

            WriteFolderIfNotExists(basedir);
            WriteFolderIfNotExists(core);
            WriteFolderIfNotExists(impl);

            WriteFile(irepository, TemplateGenarator.ReadTemplate("IRepository"));
            WriteFile(repository,TemplateGenarator.ReadTemplate("Repository"));

            foreach (var repo in repos)
            {
                WriteRepository(repo,basedir);
            }
        }
    }
}
