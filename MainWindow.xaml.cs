using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DatabaseSchemaReader;
using MyCodeGenerator.Generators;

namespace MyCodeGenerator
{
   
    public partial class MainWindow : Window
    {
        private const string providername = "System.Data.SqlClient";
        private string connectionString = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            if(ConnectionString.Text.Length<1)
                return;
            
            if(LocationInput.Text.Length<1)
                return;

            connectionString = ConnectionString.Text;
            var rootDirectory = new DirectoryInfo(LocationInput.Text);
            if(rootDirectory.Exists==false)
                rootDirectory.Create();

            var reader = new DatabaseReader(connectionString, providername);
            var schema = reader.ReadAll();
            Settings.RootDirectory = rootDirectory;
            Settings.ProjectNamespace = ProjectNamespaceInput.Text;
            Settings.ProjectName = ProjectNameInput.Text;
            MainGenerator.Generate(schema);
        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog= new FolderBrowserDialog();
            dialog.ShowDialog();
            var location = dialog.SelectedPath;
            LocationInput.Text = location;
        }

        private void ConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ProjectNameInput_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
