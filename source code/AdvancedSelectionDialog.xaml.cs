using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace RevitLinkManagerUltra
{
    public partial class AdvancedSelectionDialog : Window
    {
        public SelectionOption SelectedOption { get; private set; }
        public List<string> SelectedFiles { get; private set; }
        public string SelectedFolder { get; private set; }
        public bool IncludeSubfolders { get; private set; }

        public AdvancedSelectionDialog()
        {
            InitializeComponent();
            SelectedFiles = new List<string>();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (FileSelectionRadio.IsChecked == true)
            {
                // File selection mode
                var openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Revit Files (*.rvt)|*.rvt",
                    Title = "Select Revit Files to Link"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedFiles = openFileDialog.FileNames.ToList();
                    SelectedOption = SelectionOption.Files;
                    DialogResult = true;
                }
            }
            else
            {
                // Folder selection mode
                var folderDialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Select folder containing Revit files"
                };

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedFolder = folderDialog.SelectedPath;
                    IncludeSubfolders = IncludeSubfoldersCheckBox.IsChecked == true;
                    SelectedOption = SelectionOption.Folder;
                    DialogResult = true;
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}