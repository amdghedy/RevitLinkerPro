using System.Windows;

namespace RevitLinkManagerUltra
{
    public partial class LinkingProgressWindow : Window
    {
        private int _totalFiles;

        public LinkingProgressWindow(int totalFiles)
        {
            InitializeComponent();
            _totalFiles = totalFiles;
            ProgressBar.Maximum = totalFiles;
            ProgressBar.Value = 0;
        }

        public void UpdateProgress(int currentFile, string fileName, string status)
        {
            if (currentFile <= _totalFiles)
            {
                ProgressBar.Value = currentFile;
                StatusText.Text = status;
                CurrentFileText.Text = $"Current: {fileName}";

                // Force UI update
                this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}