using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitLinkManagerUltra
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LinkManagerUltraCommand : IExternalCommand
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
                // Show the advanced selection dialog
                var selectionDialog = new AdvancedSelectionDialog();
                if (selectionDialog.ShowDialog() == true)
                {
                    List<string> filesToLink = new List<string>();

                    if (selectionDialog.SelectedOption == SelectionOption.Files)
                    {
                        // Use file selection
                        filesToLink = selectionDialog.SelectedFiles;
                    }
                    else if (selectionDialog.SelectedOption == SelectionOption.Folder)
                    {
                        // Use folder scanning
                        filesToLink = ScanFolderForRevitFiles(selectionDialog.SelectedFolder, selectionDialog.IncludeSubfolders);
                    }

                    if (filesToLink.Count == 0)
                    {
                        TaskDialog.Show("Link Manager Ultra", "No Revit files found to link.");
                        return Result.Cancelled;
                    }

                    // Show progress window
                    var progressWindow = new LinkingProgressWindow(filesToLink.Count);
                    progressWindow.Show();

                    // Perform linking with progress tracking
                    UltraLinkResults results = PerformUltraLinking(doc, filesToLink, progressWindow);

                    progressWindow.Close();

                    // Show comprehensive results
                    ShowUltraResults(results, filesToLink.Count);

                    return Result.Succeeded;
                }

                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Link Manager Ultra - Error", $"Operation failed: {ex.Message}");
                return Result.Failed;
            }
        }

        public UltraLinkResults PerformUltraLinking(Document doc, List<string> filePaths, LinkingProgressWindow progressWindow)
        {
            UltraLinkResults results = new UltraLinkResults();
            Stopwatch totalTimer = Stopwatch.StartNew();

            using (Transaction trans = new Transaction(doc, "Ultra Multi-File Linking"))
            {
                trans.Start();

                for (int i = 0; i < filePaths.Count; i++)
                {
                    string filePath = filePaths[i];
                    string fileName = System.IO.Path.GetFileName(filePath);
                    Stopwatch fileTimer = Stopwatch.StartNew();

                    // Update progress window
                    progressWindow.UpdateProgress(i + 1, fileName, $"Linking file {i + 1} of {filePaths.Count}");

                    try
                    {
                        if (!System.IO.File.Exists(filePath))
                        {
                            results.AddFailed(fileName, "File not found", fileTimer.ElapsedMilliseconds);
                            continue;
                        }

                        // Check file size and warn if too large
                        long fileSize = new System.IO.FileInfo(filePath).Length;
                        if (fileSize > 500 * 1024 * 1024) // 500MB warning
                        {
                            results.AddWarning(fileName, "Large file (>500MB) may take longer to process");
                        }

                        // Check for duplicates
                        if (IsFileAlreadyLinked(doc, filePath))
                        {
                            results.AddSkipped(fileName, "Already linked", fileTimer.ElapsedMilliseconds);
                            continue;
                        }

                        // Create the link
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                        RevitLinkOptions options = new RevitLinkOptions(false);

                        LinkLoadResult loadResult = RevitLinkType.Create(doc, modelPath, options);

                        if (loadResult != null && loadResult.ElementId != ElementId.InvalidElementId)
                        {
                            RevitLinkInstance linkInstance = RevitLinkInstance.Create(doc, loadResult.ElementId);

                            if (linkInstance != null)
                            {
                                results.AddSuccess(fileName, fileTimer.ElapsedMilliseconds, fileSize);
                                AddEnhancedMetadata(linkInstance, fileSize);
                            }
                            else
                            {
                                results.AddFailed(fileName, "Failed to create instance", fileTimer.ElapsedMilliseconds);
                            }
                        }
                        else
                        {
                            results.AddFailed(fileName, "Invalid load result", fileTimer.ElapsedMilliseconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        results.AddFailed(fileName, ex.Message, fileTimer.ElapsedMilliseconds);
                    }
                    finally
                    {
                        fileTimer.Stop();
                    }
                }

                trans.Commit();
            }

            totalTimer.Stop();
            results.TotalTimeMs = totalTimer.ElapsedMilliseconds;
            return results;
        }

        public void ShowUltraResults(UltraLinkResults results, int totalFiles)
        {
            UltraResultsDialog resultsDialog = new UltraResultsDialog(results, totalFiles);
            resultsDialog.ShowDialog();
        }

        private List<string> ScanFolderForRevitFiles(string folderPath, bool includeSubfolders)
        {
            List<string> revitFiles = new List<string>();
            var searchOption = includeSubfolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;

            try
            {
                revitFiles.AddRange(System.IO.Directory.GetFiles(folderPath, "*.rvt", searchOption));
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Scan Error", $"Failed to scan folder: {ex.Message}");
            }

            return revitFiles;
        }

        private bool IsFileAlreadyLinked(Document doc, string filePath)
        {
            try
            {
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                var linkTypes = collector.OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>();

                foreach (RevitLinkType linkType in linkTypes)
                {
                    if (linkType.GetExternalFileReference()?.GetPath() == modelPath)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If we can't check, assume it's not linked
            }

            return false;
        }

        private void AddEnhancedMetadata(RevitLinkInstance linkInstance, long fileSize)
        {
            try
            {
                string fileSizeText = FormatFileSize(fileSize);
                string metadata = $"Linked by Link Manager Ultra on {DateTime.Now:yyyy-MM-dd HH:mm} | Size: {fileSizeText}";

                Parameter commentsParam = linkInstance.LookupParameter("Comments");
                if (commentsParam != null && !commentsParam.IsReadOnly)
                {
                    commentsParam.Set(metadata);
                }
            }
            catch
            {
                // Silent fail - metadata is optional
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}