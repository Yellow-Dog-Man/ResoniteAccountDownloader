using AccountOperationUtilities.Interfaces;
using SkyFrost.Base;
using System.Collections.Generic;

namespace ResoniteAccountDownloader.Models.Adapters
{
    public class ResoniteRecordDownloadStatus : RefreshableModel, IRecordDownloadStatus
    {
        private RecordMigrationStatus _status;
        public ResoniteRecordDownloadStatus(RecordMigrationStatus status)
        {
            _status = status;
        }

        public int AssetsToUpload { get => _status.AssetsToUpload; set { } }
        public int AssetsUploaded { get => _status.AssetsUploaded; set { } }
        public long BytesToUpload { get => _status.BytesToUpload; set { } }
        public long BytesUploaded { get => _status.BytesUploaded; set { } }
        public int DownloadedRecordCount { get => _status.TotalRecordCount; set { } }

        //TODO
        public List<IRecordDownloadFailure> FailedRecords { get => new List<IRecordDownloadFailure>(); set { } }
        public int TotalRecordCount { get => _status.TotalProcessedRecordCount; set { } }

        public string GenerateReport()
        {
            return "";
        }
    }
}
