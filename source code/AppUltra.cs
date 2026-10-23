using System;
using System.Reflection;
using Autodesk.Revit.UI;

namespace RevitLinkManagerUltra
{
    public class AppUltra : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Create ribbon tab
            string tabName = "Link Manager Ultra";
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch (Exception)
            {
                // Tab already exists
            }

            // Create ribbon panel
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Advanced Linking");

            // Create main push button
            PushButtonData mainButtonData = new PushButtonData(
                "LinkManagerUltra",
                "Link Manager\nUltra",
                Assembly.GetExecutingAssembly().Location,
                "RevitLinkManagerUltra.LinkManagerUltraCommand");

            mainButtonData.ToolTip = "Advanced multi-file linking with folder scanning and progress tracking";
            mainButtonData.LongDescription = "Link multiple Revit files with folder scanning, progress tracking, and performance analytics";

            // Create folder linking button
            PushButtonData folderButtonData = new PushButtonData(
                "LinkFoldersUltra",
                "Link from\nFolders",
                Assembly.GetExecutingAssembly().Location,
                "RevitLinkManagerUltra.LinkFromFoldersCommand");

            folderButtonData.ToolTip = "Link all Revit files from selected folders and subfolders";

            // Add buttons to panel
            panel.AddItem(mainButtonData);
            panel.AddItem(folderButtonData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}