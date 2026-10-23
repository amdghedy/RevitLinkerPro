# RevitLinkerPro — Batch Revit Link Manager

**RevitLinkerPro** is a Revit add-in for linking multiple Revit project files (`.rvt`) into the currently open host project. It reduces repetitive file-by-file work with multi-select, folder scanning, progress feedback, duplicate checks, and a per-file results summary.

> **Current implementation note:** This README documents the functionality present in the checked-in source. The current implementation creates links using Revit's default placement (**origin-to-origin**) and does not currently expose UI controls for Overlay/Attachment reference type or Room Bounding. Those settings should not be assumed to be available until they are implemented in the code.

---

## Contents

- [Features](#features)
- [Ribbon commands](#ribbon-commands)
- [Workflow](#workflow)
- [Results and diagnostics](#results-and-diagnostics)
- [Requirements](#requirements)
- [Installation](#installation)
- [Build from source](#build-from-source)
- [Important behavior and limitations](#important-behavior-and-limitations)
- [Troubleshooting](#troubleshooting)
- [Project structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Features

### Batch linking

- Select multiple individual `.rvt` files in one file-picker operation.
- Scan a selected folder for `.rvt` files.
- Choose whether a folder scan includes subfolders or only the selected top-level folder.
- Review the files found by the dedicated **Link from Folders** command before starting.
- Link the selected files into the currently active Revit project.
- Display live progress, including the current filename and item count.
- Continue processing the remaining files when an individual file fails.
- Detect files that appear to be already linked and skip them.
- Show per-file success, failure, and skipped results.
- Measure total elapsed time and average successful-link processing time.
- Warn when an input file exceeds 500 MB.
- Attempt to write optional linking metadata to the linked instance's **Comments** parameter.

### User interface

- **Link Manager Ultra** — selection workflow for individual files or folder scanning.
- **Link from Folders** — folder-based workflow with a preview list before linking.
- A progress window showing the operation status and current file.
- A results dialog with separate **Success**, **Failed**, and **Skipped** tabs.

## Ribbon commands

After installation, look for the **Link Manager Ultra** ribbon tab and its **Advanced Linking** panel.

| Command | What it does |
|---|---|
| **Link Manager Ultra** | Opens the selection dialog. Choose individual files or scan a folder, then starts the batch operation. |
| **Link from Folders** | Opens a folder picker, asks whether to include subfolders, displays the discovered files for review, and starts linking after confirmation. |

### Selection options

In **Link Manager Ultra**:

| Option | Behavior |
|---|---|
| **Select individual files** | Opens a multi-select file dialog filtered to Revit project files (`.rvt`). |
| **Scan folders and subfolders** | Select a folder and scan for `.rvt` files. |
| **Include subfolders in scan** | When enabled, searches all descendant folders; when disabled, searches only the selected folder. This checkbox is available for folder mode. |
| **Continue / Cancel** | Continue to the selected workflow or close the dialog without linking. |

In **Link from Folders**, the add-in asks whether to include subfolders, then presents a preview showing the number of files found, the selected folder, the scan scope, and the discovered filenames. Choose **Link Files** to proceed or **Cancel** to stop.

## Workflow

1. Open the **host Revit project** in which you want to place the links.
2. Open **Link Manager Ultra** from the Revit ribbon.
3. Choose **Select individual files** or **Scan folders and subfolders**.
4. Select the required files or folder and confirm the scan scope.
5. For the **Link from Folders** command, review the discovered file list and choose **Link Files**.
6. Monitor the progress window while each file is processed.
7. Review the final results, including successful, failed, and skipped files.

## Results and diagnostics

The results dialog provides a summary containing:

- Total number of files submitted for processing
- Successful, failed, and skipped counts
- Total elapsed time
- Average time per successful link

The tabs show:

- **Success** — filename and reported processing duration.
- **Failed** — filename and the error message recorded for that file.
- **Skipped** — filename and the reason it was skipped, such as an already-linked file.

A file larger than **500 MB** generates a warning during processing. The warning is recorded internally; the current results window does not have a separate Warnings tab.

## Requirements

- Windows
- Autodesk Revit
- A Revit project open as the host document
- .NET Framework **4.7** target in the checked-in project configuration
- Revit API assemblies matching the Revit version you intend to run

The project file currently references Revit API assemblies from a local **Revit 2020** installation path. That makes the checked-in project configuration Revit-2020-oriented; compatibility with other Revit releases has not been established by this repository configuration.

## Installation

The repository contains source code, a compiled DLL, and an example `.addin` manifest. The manifest currently contains a **placeholder assembly path** and must be edited for the target machine.

1. Close Revit.
2. Obtain or build `RevitLinkManagerUltra.dll`.
3. Copy the DLL to a stable local folder, for example:
   `%AppData%\Autodesk\Revit\Addins\2020\RevitLinkerPro\`
4. Copy `RevitLinkManagerUltra.addin` to the matching Revit Addins folder:
   `%AppData%\Autodesk\Revit\Addins\2020\`
5. Edit the `<Assembly>` value in the manifest so it points to the actual full path of `RevitLinkManagerUltra.dll`. Do not leave the example `C:\PATH\TO\YOUR\RevitLinkManagerUltra.dll` value unchanged.
6. Confirm the `<FullClassName>` remains `RevitLinkManagerUltra.AppUltra` and the assembly path is correct.
7. Start Revit and check for the **Link Manager Ultra** ribbon tab.

> Use the Addins folder for the Revit release you have configured and ensure the add-in was compiled against that release's API. The example path above is for Revit 2020, matching the current project references.

## Build from source

1. Install Visual Studio with the **.NET desktop development** workload and the .NET Framework 4.7 targeting pack.
2. Install the intended Revit version and locate its `RevitAPI.dll` and `RevitAPIUI.dll`.
3. Open `RevitLinkManagerUltra.sln`.
4. Check the `HintPath` values for `RevitAPI` and `RevitAPIUI` in `RevitLinkManagerUltra.csproj`. Update them to the correct local Revit installation if necessary.
5. Build the solution in the desired configuration.
6. Update the `.addin` manifest's assembly path to the built DLL and install the manifest as described above.

A successful compilation alone does not verify runtime behavior. Test in a disposable sample project and with a small set of local RVT files before using it on production models.

## Important behavior and limitations

- **Placement:** the current linking routine creates link instances at Revit's default **origin-to-origin** placement. Shared-coordinate placement is not exposed by the current UI.
- **Reference type and Room Bounding:** Overlay/Attachment and Room Bounding controls are not implemented in the checked-in linking routine.
- **File types:** folder scans search for `*.rvt` files only. Revit family files (`.rfa`) are not included.
- **Duplicate check:** the tool attempts to compare the external reference path to the selected path. Path normalization differences may affect duplicate detection.
- **Large files:** files over 500 MB are warned about, not automatically rejected.
- **Transactions:** the batch is performed inside one Revit transaction. If Revit enters a failure state during processing, the overall transaction may not behave as a per-file independent commit.
- **Metadata:** the add-in attempts to write a timestamp and file size to the link instance's `Comments` parameter. This is optional and can silently fail if the parameter is unavailable or read-only.
- **Performance:** linking time depends on file size, model complexity, storage speed, network access, and Revit's processing.
- **Workshared/cloud models:** the checked-in code is primarily based on local file paths. Validate the workflow with your actual model-storage setup before relying on it for cloud-hosted or workshared projects.
- **Production safety:** make a backup and test on a copy of the host model before batch linking important project data.

## Troubleshooting

| Problem | Things to check |
|---|---|
| Ribbon tab does not appear | Confirm the `.addin` file is in the correct Revit-version Addins folder and that its XML is valid. Restart Revit. |
| Add-in fails to load | Verify the assembly path, DLL availability, .NET target, and matching Revit API references. |
| “Please open a Revit project first” | Open a project document and retry. |
| No files are found | Confirm the folder contains `.rvt` files and check whether subfolders are included. |
| A file is skipped as already linked | Review the host model's existing Revit links and confirm the selected path is the intended file. |
| A file fails to link | Check that the file exists and can be accessed, that it is a valid Revit project, and that Revit reports no file or path issue. Review the error shown in the Failed tab. |
| Folder scan reports an error | Confirm folder permissions and that the folder is reachable. Network and inaccessible subfolders can cause scanning errors. |

## Project structure

| File | Purpose |
|---|---|
| `AppUltra.cs` | Registers the ribbon tab, panel, and two commands. |
| `LinkManagerUltraCommand.cs` | Main selection flow, batch linking, duplicate detection, progress updates, and optional metadata. |
| `LinkFromFoldersCommand.cs` | Folder selection, recursive-scan choice, preview, and batch launch. |
| `AdvancedSelectionDialog.xaml(.cs)` | File-versus-folder selection UI. |
| `FolderScanPreviewDialog.xaml(.cs)` | Folder scan summary and filename preview. |
| `LinkingProgressWindow.xaml(.cs)` | Progress and current-file display. |
| `UltraResultsDialog.xaml(.cs)` | Success, failure, skipped, and timing summary UI. |
| `UltraLinkResults.cs` | Stores per-file results, counts, warnings, file sizes, and timing data. |
| `RevitLinkManagerUltra.csproj` | Build configuration and Revit API references. |
| `RevitLinkManagerUltra.addin` | Revit add-in manifest example; edit the assembly path before use. |

## Contributing

Issues and pull requests are welcome. When reporting a problem, include:

- Revit version and Windows version
- Whether files are local, network-hosted, or cloud-hosted
- Steps to reproduce the issue
- The relevant error text from the results dialog
- Whether the issue occurs with a single file or a batch

Please do not upload confidential project models or credentials to public issues.

## License

No license file is currently included in this repository. Unless a license is added, reuse and redistribution of this source code are not automatically granted.
