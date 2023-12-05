using AccountOperationUtilities.Interfaces;
using SkyFrost.Base;

namespace ResoniteAccountDownloader.Models.Adapters
{
    public class ResoniteGroupDownloadStatus : IGroupDownloadStatus
    {
        private GroupMigrationStatus _Status;
        private ResoniteRecordDownloadStatus _Records;
        private ResoniteVariableDownloadStatus _Variables;
        public ResoniteGroupDownloadStatus(GroupMigrationStatus status) {
            _Status = status;
            _Records = new ResoniteRecordDownloadStatus(_Status.RecordsStatus);
            _Variables = new ResoniteVariableDownloadStatus(_Status.VariablesStatus);
        }

        public string OwnerId { get => _Status.OwnerId; set {} }
        public string GroupName { get => _Status.GroupName; set {} }
        public int DownloadedMemberCount { get => _Status.MigratedMemberCount; set {} }
        public IRecordDownloadStatus RecordsStatus { get => _Records; set {} }
        public IVariableDownloadStatus VariablesStatus { get => _Variables; set {} }
    }
}
