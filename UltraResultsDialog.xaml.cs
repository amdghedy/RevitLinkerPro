using System.Windows;
using System.Linq;

namespace RevitLinkManagerUltra
{
    public partial class UltraResultsDialog : Window
    {
        public UltraResultsDialog(UltraLinkResults results, int totalFiles)
        {
            InitializeComponent();
            DisplayResults(results, totalFiles);
        }

        private void DisplayResults(UltraLinkResults results, int totalFiles)
        {
            // Set title and summary
            if (results.SuccessCount == totalFiles)
            {
                ResultsTitle.Text = "✅ All Files Linked Successfully!";
            }
            else if (results.SuccessCount > 0)
            {
                ResultsTitle.Text = "⚠️ Partial Success";
            }
            else
            {
                ResultsTitle.Text = "❌ Linking Failed";
            }

            SummaryText.Text = $"Total Files: {totalFiles} | " +
                             $"Successful: {results.SuccessCount} | " +
                             $"Failed: {results.FailedCount} | " +
                             $"Skipped: {results.SkippedCount}\n" +
                             $"Total Time: {results.TotalTimeMs / 1000.0:F2} seconds | " +
                             $"Average Time: {results.AverageSuccessTimeMs:F0} ms per file";

            // Populate lists
            SuccessList.ItemsSource = results.SuccessFiles.Select(f =>
                $"{f.FileName} ({f.DurationSeconds}s)");

            FailedList.ItemsSource = results.FailedFiles.Select(f =>
                $"{f.FileName}: {f.Error}");

            SkippedList.ItemsSource = results.SkippedFiles.Select(f =>
                $"{f.FileName}: {f.Error}");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}