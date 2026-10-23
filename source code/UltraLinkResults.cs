using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitLinkManagerUltra
{
    public class UltraLinkResults
    {
        public List<FileResult> SuccessFiles { get; set; } = new List<FileResult>();
        public List<FileResult> FailedFiles { get; set; } = new List<FileResult>();
        public List<FileResult> SkippedFiles { get; set; } = new List<FileResult>();
        public List<string> Warnings { get; set; } = new List<string>();
        public long TotalTimeMs { get; set; }
        public long TotalFilesSize { get; set; }

        public int SuccessCount => SuccessFiles.Count;
        public int FailedCount => FailedFiles.Count;
        public int SkippedCount => SkippedFiles.Count;
        public int TotalProcessed => SuccessCount + FailedCount + SkippedCount;

        public void AddSuccess(string fileName, long durationMs, long fileSize)
        {
            SuccessFiles.Add(new FileResult
            {
                FileName = fileName,
                DurationMs = durationMs,
                FileSize = fileSize
            });
            TotalFilesSize += fileSize;
        }

        public void AddFailed(string fileName, string error, long durationMs)
        {
            FailedFiles.Add(new FileResult
            {
                FileName = fileName,
                Error = error,
                DurationMs = durationMs
            });
        }

        public void AddSkipped(string fileName, string reason, long durationMs)
        {
            SkippedFiles.Add(new FileResult
            {
                FileName = fileName,
                Error = reason,
                DurationMs = durationMs
            });
        }

        public void AddWarning(string fileName, string warning)
        {
            Warnings.Add($"{fileName}: {warning}");
        }

        public double AverageSuccessTimeMs => SuccessFiles.Count > 0 ? SuccessFiles.Average(f => f.DurationMs) : 0;
    }

    public class FileResult
    {
        public string FileName { get; set; }
        public string Error { get; set; }
        public long DurationMs { get; set; }
        public long FileSize { get; set; }
        public double DurationSeconds => Math.Round(DurationMs / 1000.0, 2);
    }

    public enum SelectionOption
    {
        Files,
        Folder
    }
}