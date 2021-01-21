using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace CompareJsonIhm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Editor1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Editor2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Editor3_TextChanged(object sender, EventArgs e)
        {

        }



        private void BtnOpenFile1_Click(object sender, RoutedEventArgs e)
        {
            LabelFile1.Content = GetFileFromOpenFile(LabelFile1.Content?.ToString());
            Editor1.Load(LabelFile1.Content.ToString());
        }

        private void BtnOpenFile2_Click(object sender, RoutedEventArgs e)
        {
            LabelFile2.Content = GetFileFromOpenFile(LabelFile2.Content.ToString());
            Editor2.Load(LabelFile2.Content.ToString());
        }

        private void BtnOpenFile3_Click(object sender, RoutedEventArgs e)
        {
            LabelFile3.Content = GetFileFromOpenFile(LabelFile3.Content.ToString());
            Editor3.Load(LabelFile3.Content.ToString());
        }



        private void BtnSaveFile1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveFile2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveFile3_Click(object sender, RoutedEventArgs e)
        {

        }



        private string GetFileFromOpenFile(string file)
        {

            string initialFilename = string.Empty;
            string initialDirectory = string.Empty;

            if (!string.IsNullOrEmpty(file))
            {
                var initialFile = new System.IO.FileInfo(file);
                if (initialFile.Exists)
                    initialFilename = initialFile.FullName;

                if (initialFile.Directory.Exists)
                    initialDirectory = initialFile.Directory.FullName;

            }
            else
            {
                initialDirectory = Environment.CurrentDirectory;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                FileName = initialFilename,
                InitialDirectory = initialDirectory,
                Filter = "json files (*.json)|*.json|javascript files (*.js)|*.js|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            };

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return string.Empty;

        }

        private string GetFileFromSaveFile()
        {

            //string initialFilename = string.Empty;
            //string initialDirectory = string.Empty;

            //if (!string.IsNullOrEmpty(file))
            //{
            //    var initialFile = new System.IO.FileInfo(file);
            //    if (initialFile.Exists)
            //        initialFilename = initialFile.FullName;

            //    if (initialFile.Directory.Exists)
            //        initialDirectory = initialFile.Directory.FullName;

            //}

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "",          // initialFilename,
                InitialDirectory = "",  // initialDirectory,
                Filter = "json files (*.json)|*.json|javascript files (*.js)|*.js|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            };

            if (saveFileDialog.ShowDialog() == true)
                return saveFileDialog.FileName;

            return string.Empty;

        }


    }
}
