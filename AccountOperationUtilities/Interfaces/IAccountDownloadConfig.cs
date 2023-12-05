using System.ComponentModel;

namespace AccountOperationUtilities.Interfaces;

public interface IAccountDownloadConfig : INotifyPropertyChanged
{
    bool DownloadCloudVariableDefinitions { get; set; }
    bool DownloadCloudVariables { get; set; }
    bool DownloadContacts { get; set; }
    bool DownloadGroups { get; set; }
    bool DownloadMessageHistory { get; set; }
    bool DownloadUserMetadata { get; set; }
    bool DownloadUserRecords { get; set; }

    bool DownloadExitMessages { get; set; }

    bool DownloadFundingEvents { get; set; }
    bool DownloadRecordAuditLog { get; set; }

    bool ForceOverwrite { get; set; }
    IEnumerable<string> GroupsToDownload { get; }
    bool OnlyNewRecords { get; set; }
    IEnumerable<string> RecordsToDownload { get; }
    IEnumerable<string> VariablesToDownload { get; }

    public string FilePath { get; }

    public int MaxDegreeOfParallelism { get; set; }
}
