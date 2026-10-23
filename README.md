# Revit Linker Pro — Link Manager Ultra

A Revit add-in for linking multiple external RVT files into the currently open host project, either by selecting individual files or scanning a folder.

## Compatibility and prerequisites

The repository contains a C# Revit add-in with WPF dialogs and Revit API calls. Confirm the target Revit version and .NET Framework in the project configuration before building; this README intentionally does not claim cross-version compatibility without a verified project target.

Requirements:
- Windows and a supported Revit installation
- Visual Studio with the appropriate .NET desktop development workload
- Matching Revit API references for the target Revit version
- Read access to the RVT files to link and write access to the host model

## Ribbon commands

The add-in registers a ribbon tab named **Link Manager Ultra**, with an **Advanced Linking** panel.

| Command | Purpose |
|---|---|
| **Link Manager Ultra** | Opens the advanced selection workflow, allowing individual RVT files or a folder scan. |
| **Link from Folders** | Selects a folder, optionally scans subfolders, previews discovered files, then links the selected batch. |

## Features and options

- **Multi-file selection:** choose multiple .rvt files in the file picker.
- **Folder scanning:** scan only the selected folder or include subfolders.
- **Preview before linking:** review the discovered RVT filenames and count before confirming.
- **Progress window:** shows current file and batch progress.
- **Duplicate check:** files already linked are skipped by the linking logic.
- **Per-file result tracking:** successful, skipped, warning, and failed items are recorded in the result summary.
- **Large-file warning:** files over 500 MB trigger a warning that processing may take longer.
- **Timing information:** the command records per-file elapsed time and total operation time.
- **Enhanced metadata:** the implementation attaches additional metadata to created link instances.

The add-in creates Revit link types and link instances inside a Revit transaction. It does not merge the linked model's elements into the host model.

## Quick start: link a batch

1. Open the **host Revit project** that should contain the links.
2. Launch **Link Manager Ultra** from the ribbon.
3. Choose file selection or folder selection.
4. For file selection, pick one or more RVT files. For folder selection, choose a directory and decide whether subfolders should be included.
5. Review the scan preview and file count.
6. Confirm the operation to begin linking.
7. Monitor the progress window.
8. Review the final result summary for successes, skipped duplicates, warnings, and failures.
9. Inspect link placement, coordinates, visibility, and path behavior in the host model.
10. Save the host model after validating the result.

### Folder workflow

The **Link from Folders** command asks whether to include subfolders, scans for .rvt files, sorts the file list, displays a preview, and only proceeds if the preview is accepted.

## Build and install

1. Inspect the project/solution files at the repository root to identify the configured Revit version, target framework, and API reference paths.
2. Install that exact Revit version and its matching API assemblies.
3. Open the solution in Visual Studio.
4. Repair any API references that point to another machine's installation path.
5. Build the project.
6. Locate the compiled DLL and verify the .addin manifest points to it.
7. Close Revit, deploy the DLL and manifest to the version-specific per-user folder:

   **%APPDATA%\Autodesk\Revit\Addins\[RevitYear]\**

8. Start Revit and confirm the **Link Manager Ultra** ribbon tab appears.

Do not copy a DLL compiled for one Revit API year into another year's Addins folder.

## Troubleshooting

- **Ribbon tab missing:** check the .addin manifest, assembly path, startup class, and target-version folder.
- **No files found:** verify the selected directory contains accessible .rvt files; confirm the subfolder option.
- **File skipped as duplicate:** inspect existing Revit links in the host project before retrying.
- **A link fails:** verify the path is valid, the file is readable, and the model is compatible with the host Revit version.
- **Slow linking:** large RVT files may take longer; review the size warning and process a smaller test batch first.
- **Unexpected placement:** verify shared coordinates, origin-to-origin expectations, and host project coordinate conventions.
- **Transaction failure:** save a copy of the host model before retrying and inspect the result summary/error details.

## Safety and limitations

- Test on a copy of the host model before linking a large set.
- Verify link placement and coordinate alignment after linking; this documentation does not imply automatic shared-coordinate alignment.
- Folder scanning discovers RVT files recursively only when that option is selected.
- Compatibility depends on the actual target framework and Revit API references in this repository.
