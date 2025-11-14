using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitLinkManagerUltra
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LinkFromFoldersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            if (doc == null)
            {
                TaskDialog.Show("Link Manager Ultra", "Please open a Revit project first.");
                return Result.Cancelled;
            }

            try
            {
                // Show folder browser dialog
                using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "Select folder containing Revit files to link";
                    folderDialog.ShowNewFolderButton = false;

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Ask about subfolders
                        var subfolderResult = TaskDialog.Show("Link Manager Ultra",
                            "Include subfolders in the scan?",
                            TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No | TaskDialogCommonButtons.Cancel);

                        if (subfolderResult == TaskDialogResult.Cancel)
                            return Result.Cancelled;

                        bool includeSubfolders = (subfolderResult == TaskDialogResult.Yes);

                        // Scan for Revit files
                        List<string> revitFiles = ScanFolderForRevitFiles(folderDialog.SelectedPath, includeSubfolders);

                        if (revitFiles.Count == 0)
                        {
                            TaskDialog.Show("Link Manager Ultra", "No Revit files found in the selected folder.");
                            return Result.Cancelled;
                        }

                        // Show file preview before linking
                        var previewDialog = new FolderScanPreviewDialog(revitFiles, folderDialog.SelectedPath, includeSubfolders);
                        if (previewDialog.ShowDialog() == true)
                        {
                            // Show progress window
                            var progressWindow = new LinkingProgressWindow(revitFiles.Count);
                            progressWindow.Show();

                            // Perform linking using main command's method
                            var mainCommand = new LinkManagerUltraCommand();
                            UltraLinkResults results = mainCommand.PerformUltraLinking(doc, revitFiles, progressWindow);

                            progressWindow.Close();

                            // Show results using main command's method
                            mainCommand.ShowUltraResults(results, revitFiles.Count);

                            return Result.Succeeded;
                        }
                    }
                }

                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Link Manager Ultra - Error", $"Folder scanning failed: {ex.Message}");
                return Result.Failed;
            }
        }

        private List<string> ScanFolderForRevitFiles(string folderPath, bool includeSubfolders)
        {
            List<string> revitFiles = new List<string>();
            var searchOption = includeSubfolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;

            try
            {
                revitFiles.AddRange(System.IO.Directory.GetFiles(folderPath, "*.rvt", searchOption));

                // Sort by filename for better organization
                revitFiles.Sort();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Scan Error", $"Failed to scan folder: {ex.Message}");
            }

            return revitFiles;
        }
    }
}