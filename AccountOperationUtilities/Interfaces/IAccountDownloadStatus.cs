using System.ComponentModel;

namespace AccountOperationUtilities.Interfaces;
public interface IAccountDownloadStatus : INotifyPropertyChanged
{
    List<IAssetFailure> AssetFailures { get; set; }
    int AssetsSkipped { get; set; }
    DateTimeOffset? CompletedOn { get; set; }
    string CurrentlyDownloadingItem { get; set; }
    string CurrentlyDownloadingName { get; set; }
    int DownloadedContactCount { get; set; }
    int DownloadedGroupCount { get; set; }
    int DownloadedMessageCount { get; set; }
    string Error { get; set; }
    List<IGroupDownloadStatus> GroupStatuses { get; set; }
    float PercentageContacts { get; }
    float PercentageGroups { get; }
    float PercentageRecords { get; }
    string Phase { get; set; }
    double RecordsPerMinute { get; set; }
    DateTimeOffset? StartedOn { get; set; }
    int TotalAssetCount { get; set; }
    int TotalContactCount { get; set; }
    int TotalDownloadedAssetCount { get; set; }
    int TotalDownloadedMemberCount { get; }
    int TotalDownloadedRecordCount { get; set; }
    int TotalDownloadedVariableCount { get; }
    int TotalDownloadedVariableDefinitionCount { get; }
    int TotalFailedRecordCount { get; }
    int TotalGroupCount { get; set; }
    int TotalRecordCount { get; set; }
    TimeSpan? TotalTime { get; }
    IRecordDownloadStatus UserRecordsStatus { get; set; }
    IVariableDownloadStatus UserVariablesStatus { get; set; }

    string GenerateReport();
    IGroupDownloadStatus GetGroupStatus(string ownerId, string groupName);
    void RegisterAssetFailure(IAssetFailure failure);
    void UpdateStats();
}
