# RevitLinkerPro — Batch Revit Link Manager

RevitLinkerPro is a Revit add-in for batch-linking Revit project files (`.rvt`) into the currently open host project. It provides file selection, folder scanning, progress feedback, duplicate checks, and a per-file result summary.

## Repository layout

```text
RevitLinkerPro/
├── 00-Revit Addin/
│   ├── Revit 2020/  (compiled DLL supplied)
│   ├── Revit 2021/  (manifest template; build required)
│   ├── Revit 2022/  (manifest template; build required)
│   ├── Revit 2023/  (manifest template; build required)
│   ├── Revit 2024/  (manifest template; build required)
│   ├── Revit 2025/  (manifest template; build required)
│   └── Revit 2026/  (manifest template; build required)
├── source code/
│   ├── RevitLinkManagerUltra.sln
│   ├── RevitLinkManagerUltra.csproj
│   ├── *.cs
│   ├── *.xaml
│   └── Properties/
└── README.md
```

**Version status:** Version-specific deployment folders and manifest templates are now provided for Revit 2020–2026. The repository currently contains a compiled DLL only for Revit 2020. The Revit 2021–2026 folders are prepared for their matching builds, but their DLLs have not been compiled or runtime-tested here. Do not copy the 2020 DLL into newer-version folders and assume compatibility. A manifest is not itself a compiled add-in.

## Download

### Get the available add-in files

1. Open the [00-Revit Addin folder](https://github.com/amdghedy/RevitLinkerPro/tree/main/00-Revit%20Addin/Revit%202020).
2. Open `RevitLinkManagerUltra.dll` and use GitHub's **Download raw file** control (or use the direct file link below).
3. Download `RevitLinkManagerUltra.addin` from the same folder.
4. Save both files locally. The DLL is the add-in assembly loaded by Revit; it is **not a standalone Windows application**.

- [Download RevitLinkManagerUltra.dll](https://raw.githubusercontent.com/amdghedy/RevitLinkerPro/main/00-Revit%20Addin/Revit%202020/RevitLinkManagerUltra.dll)
- [Download the Revit 2020 manifest](https://raw.githubusercontent.com/amdghedy/RevitLinkerPro/main/00-Revit%20Addin/Revit%202020/RevitLinkManagerUltra.addin)
- [Browse all add-in files](https://github.com/amdghedy/RevitLinkerPro/tree/main/00-Revit%20Addin)

### Download all source code

1. Open the [repository page](https://github.com/amdghedy/RevitLinkerPro).
2. Select **Code**.
3. Select **Download ZIP**.
4. Extract the ZIP, then open `source code/RevitLinkManagerUltra.sln` in Visual Studio.

## Is there an EXE?

**No standalone `.exe` is included in this repository at present.** Revit add-ins are normally compiled as DLL assemblies and loaded by Revit through an `.addin` manifest. The supplied `RevitLinkManagerUltra.dll` is the executable add-in component; it is not launched by double-clicking it.

An installer/launcher EXE would be a separate deliverable and must be created and tested separately. No installer EXE download link is provided until such a file is actually built and published. Avoid downloading unofficial repackaged executables.

## Features

- Select multiple individual `.rvt` files in one file-picker operation.
- Scan a selected folder for `.rvt` files.
- Choose whether to include subfolders or scan only the selected folder.
- Preview discovered files in the **Link from Folders** workflow.
- Link selected files into the active Revit host project.
- Show progress and the current filename during processing.
- Continue processing after an individual file fails.
- Detect files that appear to be already linked and skip them.
- Display **Success**, **Failed**, and **Skipped** results.
- Report total elapsed time and average successful-link processing time.
- Warn when an input file exceeds 500 MB.
- Attempt to write timestamp and file-size metadata to the link instance's Comments parameter.

## Ribbon commands

After installation, find the **Link Manager Ultra** ribbon tab and **Advanced Linking** panel.

| Command / option | Function |
|---|---|
| **Link Manager Ultra** | Opens the selection dialog for individual files or folder scanning. |
| **Link from Folders** | Opens a folder picker, asks about subfolders, previews found files, and starts linking after confirmation. |
| **Select individual files** | Multi-selects Revit project files (`.rvt`). |
| **Scan folders and subfolders** | Selects a folder and scans for `.rvt` files. |
| **Include subfolders in scan** | Searches descendant folders when enabled; otherwise scans only the selected folder. |
| **Continue / Cancel** | Continues with the selected workflow or closes the dialog. |
| Progress window | Shows the current operation and file being processed. |
| Results dialog | Shows successful, failed, and skipped files and timing information. |

## Requirements and version support

- Windows and Autodesk Revit.
- An open Revit project to act as the host.
- .NET Framework 4.7 targeting pack for building the checked-in project.
- Revit API assemblies matching the target Revit version.

| Revit version | Repository status |
|---|---|
| Revit 2020 | Compiled DLL and manifest supplied; test in your environment before production use. |
| Revit 2021–2024 | Version folder and manifest template supplied; compile against that release's Revit API before installing. |
| Revit 2025–2026 | Version folder and manifest template supplied; a separate modern .NET build is required. The current .NET Framework project is not a ready-to-use build for these releases. |

The current project file targets Revit 2020 API assemblies. The source project uses the standard `$(ProgramFiles)\Autodesk\Revit 2020\` reference location; adjust it if Revit is installed elsewhere.

## Version-specific folders (Revit 2020–2026)

The `00-Revit Addin` directory contains a folder for each release from Revit 2020 through Revit 2026. Each folder contains the appropriate `.addin` manifest template. Only Revit 2020 currently includes a compiled DLL.

Before installing any version from 2021 onward:
1. Build the source against the matching Revit API and the correct .NET target for that release.
2. Copy that release's own DLL into its matching folder.
3. Edit that folder's manifest so `<Assembly>` points to the actual local DLL path.
4. Install the manifest in the matching version's Addins directory.

Revit 2025 and later use the modern .NET runtime, unlike the .NET Framework target in the current legacy project. They require project/build configuration changes and should not be considered supported until compiled and tested.

## Installation (Revit 2020)

1. Close Revit.
2. Create a stable local folder, for example `%AppData%\Autodesk\Revit\Addins\2020\RevitLinkerPro\`.
3. Copy `RevitLinkManagerUltra.dll` into that folder.
4. Open the downloaded `RevitLinkManagerUltra.addin` in a text editor.
5. Update its `<Assembly>` element to the full local path of `RevitLinkManagerUltra.dll`. Do not leave a GitHub URL or an example path in this field.
6. Copy the edited `.addin` manifest to `%AppData%\Autodesk\Revit\Addins\2020\` (the manifest belongs directly in that version's Addins folder).
7. Start Revit 2020 and look for the **Link Manager Ultra** ribbon tab.

The manifest's assembly path must point to a local DLL. Revit does not load the DLL directly from a GitHub URL.

## Build from source

1. Install Visual Studio with the **.NET desktop development** workload and the .NET Framework 4.7 targeting pack.
2. Install Revit 2020 or obtain matching Revit 2020 API assemblies.
3. Open `source code/RevitLinkManagerUltra.sln`.
4. Confirm the Revit API reference paths in `RevitLinkManagerUltra.csproj`.
5. Build the solution.
6. Install the newly built DLL and configure the `.addin` manifest as described above.

A successful compilation does not prove runtime compatibility. Test in a disposable sample project with a small set of RVT files before using production models.

## Behavior and limitations

- **Placement:** links use Revit's default origin-to-origin placement; shared-coordinate placement is not exposed in the current UI.
- **Reference type / Room Bounding:** Overlay/Attachment and Room Bounding controls are not implemented in the current code.
- **File types:** folder scans find `.rvt` files only, not `.rfa` families.
- **Duplicate detection:** compares external reference paths; path-normalization differences can affect detection.
- **Large files:** files over 500 MB generate a warning rather than being rejected.
- **Transactions:** the batch runs inside one Revit transaction, so individual files are not necessarily committed independently.
- **Metadata:** writing to Comments is optional and can fail if the parameter is unavailable or read-only.
- **Cloud/workshared models:** validate the workflow for your model storage environment before relying on it.
- **Safety:** back up the host model and test on a copy before batch linking important project data.

## Troubleshooting

| Issue | Checks |
|---|---|
| Ribbon tab does not appear | Check the manifest location, XML validity, assembly path, and restart Revit. |
| Add-in fails to load | Confirm the DLL exists locally and the API references and Revit version match. |
| No files found | Confirm the folder contains `.rvt` files and review the subfolder setting. |
| A file is skipped | Check whether it is already linked and verify the selected path. |
| A file fails | Check access permissions, path validity, file integrity, and the error in the Failed tab. |

## Source file guide

| File | Purpose |
|---|---|
| `AppUltra.cs` | Registers the ribbon tab, panel, and commands. |
| `LinkManagerUltraCommand.cs` | Main selection flow, batch linking, duplicate checks, progress, and metadata. |
| `LinkFromFoldersCommand.cs` | Folder selection, scan scope, preview, and batch launch. |
| `AdvancedSelectionDialog.xaml(.cs)` | File/folder selection UI. |
| `FolderScanPreviewDialog.xaml(.cs)` | Folder scan summary and filename preview. |
| `LinkingProgressWindow.xaml(.cs)` | Progress UI. |
| `UltraResultsDialog.xaml(.cs)` | Success/failure/skipped results UI. |
| `UltraLinkResults.cs` | Stores per-file results and timing data. |
| `RevitLinkManagerUltra.csproj` | Build configuration and Revit API references. |
| `RevitLinkManagerUltra.addin` | Revit manifest template; update its assembly path before use. |

## License

No license file is currently included. Unless a license is added, reuse and redistribution are not automatically granted.
