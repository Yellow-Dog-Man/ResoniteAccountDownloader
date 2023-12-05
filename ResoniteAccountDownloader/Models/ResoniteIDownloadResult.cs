using AccountOperationUtilities.Interfaces;
using System;

namespace ResoniteAccountDownloader.Models;

public class ResoniteIDownloadResult : IDownloadResult
{
    public DownloadResultType Result { get; set; }

    public string? Error { get; set; }

    public Exception? Exception { get; set; }
}
