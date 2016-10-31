using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            MainGenerator.Generate(schema);
        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog= new FolderBrowserDialog();
            dialog.ShowDialog();
            var location = dialog.SelectedPath;
            LocationInput.Text = location;
        }
    }
}
