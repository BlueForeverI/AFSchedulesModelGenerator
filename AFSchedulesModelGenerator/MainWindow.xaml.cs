using AFSchedulesModelGenerator.Helpers;
using AFSchedulesModelGenerator.Models;
using Microsoft.Win32;
using ScintillaNET;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AFSchedulesModelGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            textEditor.ConfigurationManager.Language = "cs";
            textEditor.Margins[0].Width = 50;
            textEditor.LineWrapping.Mode = LineWrappingMode.Word;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                txtFilePath.Text = dialog.FileName;
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string path = txtFilePath.Text;
            int startIndex = int.Parse(txtStartFrom.Text);
            string prefix = txtPrefix.Text;
            XmlProperty.PropertyPrefix = txtPropertyPrefix.Text.Trim();
            CsvReaderHelper.UseTaxonomy = radioUseTaxonomy.IsChecked ?? true;

            string classContent = CsvReaderHelper.ConvertFileToCode(path, prefix, startIndex);
            textEditor.Text = classContent;
        }
    }
}
