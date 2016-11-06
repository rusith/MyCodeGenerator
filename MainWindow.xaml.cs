using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using MyCodeGenerator.Generators;
using WinForms = System.Windows.Forms;

namespace MyCodeGenerator
{
   
    public partial class MainWindow 
    {
        private const string Providername = "System.Data.SqlClient";
        private string _connectionString = "";

        public MainWindow()
        {
            InitializeComponent();

            Settings.Read();
            LocationInput.Text=Settings.RootDirectory!=null? Settings.RootDirectory.FullName:"";
            ProjectNamespaceInput.Text=Settings.ProjectNamespace;
            ProjectNameInput.Text=Settings.ProjectName;
            ConnectionString.Text=Settings.ConnectionString;
            
        }

        private void Generate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString.Text))
            {
                WinForms.MessageBox.Show("Cannot Continue without a connection string", "No connection string", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(LocationInput.Text))
            {
                WinForms.MessageBox.Show("Cannot continue without root folder.", "No root folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProjectNameInput.Text))
            {
                WinForms.MessageBox.Show("Cannot continue without a project name.", "No project name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProjectNamespaceInput.Text))
            {
                WinForms.MessageBox.Show("Cannot continue without a project namespace.", "No project namespace", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _connectionString = ConnectionString.Text;
            var rootDirectory = new DirectoryInfo(LocationInput.Text);
            try
            {
                if (rootDirectory.Exists == false)
                    rootDirectory.Create();
            }
            catch (Exception)
            {
                WinForms.MessageBox.Show("The root folder does not exists and, unable to create it", "unable to create root folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DatabaseSchema schema = null;
            try
            {
                var reader = new DatabaseReader(_connectionString, Providername);
                schema = reader.ReadAll();
            }
            catch (Exception e)
            {
                WinForms.MessageBox.Show("Unable to read the database \n\n"+e.Message.Replace(Environment.NewLine,"\t"+Environment.NewLine),"Unable to read database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
            Settings.RootDirectory = rootDirectory;
            Settings.ProjectNamespace = ProjectNamespaceInput.Text;
            Settings.ProjectName = ProjectNameInput.Text;
            Settings.ConnectionString = ConnectionString.Text;
            Settings.Save();

            try
            {
                MainGenerator.Generate(schema);
              
            }
            catch (Exception e)
            {
                WinForms.MessageBox.Show("Unable to generate code \n\n" + e.Message.Replace(Environment.NewLine, "\t" + Environment.NewLine), "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
           Generate();
        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog= new FolderBrowserDialog();
            dialog.ShowDialog();
            var location = dialog.SelectedPath;
            LocationInput.Text = location;
        }

        private void OnCleanButtonClick(object sender, RoutedEventArgs e)
        {
            TemplateGenarator.Clear();
            if (string.IsNullOrWhiteSpace(LocationInput.Text))
                WinForms.MessageBox.Show("Please select a root directory", "no root directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var directory = new DirectoryInfo(LocationInput.Text);
                if (!directory.Exists)
                    WinForms.MessageBox.Show("Directory not exists select a root directory", "no root directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    Action<string> deleteFolder = pathPart =>
                    {
                        if (Directory.Exists(directory.FullName + "\\" + pathPart)) Directory.Delete(directory.FullName + "\\" + pathPart,true);
                    };
                    deleteFolder("base");
                    deleteFolder("Objects");
                    deleteFolder("Repositories");
                    TemplateGenarator.Clear();
                }
            }
        }
    }
}
