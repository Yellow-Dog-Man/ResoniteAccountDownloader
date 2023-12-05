using System.ComponentModel;

namespace AccountOperationUtilities.Interfaces;

public interface IRecordDownloadStatus : INotifyPropertyChanged
{
    int AssetsToUpload { get; set; }
    int AssetsUploaded { get; set; }
    long BytesToUpload { get; set; }
    long BytesUploaded { get; set; }
    int DownloadedRecordCount { get; set; }
    List<IRecordDownloadFailure> FailedRecords { get; set; }
    int TotalRecordCount { get; set; }

    string GenerateReport();
}
