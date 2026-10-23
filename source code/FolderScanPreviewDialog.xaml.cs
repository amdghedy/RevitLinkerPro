using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RevitLinkManagerUltra
{
    public partial class FolderScanPreviewDialog : Window
    {
        public List<string> FilesToLink { get; private set; }

        public FolderScanPreviewDialog(List<string> files, string folderPath, bool includeSubfolders)
        {
            InitializeComponent();
            FilesToLink = files;

            // Update UI
            SummaryText.Text = $"{files.Count} files found in: {folderPath}\n" +
                             $"{(includeSubfolders ? "Including subfolders" : "Top folder only")}";

            // Display file names
            FilesListBox.ItemsSource = files.Select(f => System.IO.Path.GetFileName(f));
        }

        private void LinkFiles_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}