using AccountOperationUtilities.Interfaces;
using ResoniteAccountDownloader.Models.Adapters;
using SkyFrost.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResoniteAccountDownloader.Models;

public class ResoniteDownloadStatus : RefreshableModel, IAccountDownloadStatus
{
    private AccountMigrationStatus _status;
    public ResoniteDownloadStatus(AccountMigrationStatus status)
    {
        _status = status;
        _UserVariablesStatus = new ResoniteVariableDownloadStatus(_status.UserVariablesStatus);
        _UserRecordsStatus = new ResoniteRecordDownloadStatus(_status.UserRecordsStatus);
    }

    public int AssetsSkipped { get => _status.MissingAssets.Count; set { } }
    public DateTimeOffset? CompletedOn { get => _status.CompletedOn; set { } }
    public string CurrentlyDownloadingItem { get => _status.CurrentlyMigratingItem; set { } }
    public string CurrentlyDownloadingName { get => _status.CurrentlyMigratingName; set { } }
    public int DownloadedContactCount { get => _status.MigratedContactCount; set { } }
    public int DownloadedGroupCount { get => _status.MigratedContactCount; set { } }
    public int DownloadedMessageCount { get => _status.MigratedMessageCount; set { } }
    public string Error { get => _status.Error; set { } }
    public int TotalContactCount { get => _status.TotalContactCount; set { } }


    public string Phase { get => _status.Phase; set { } }
    public double RecordsPerMinute { get => _status.RecordsPerMinute; set { } }
    public DateTimeOffset? StartedOn { get => _status.StartedOn; set { } }
    public int TotalDownloadedMemberCount => _status.TotalMigratedMemberCount;
    public int TotalDownloadedRecordCount { get => _status.TotalRecordCount; set { } }
    public int TotalDownloadedVariableCount => _status.TotalMigratedVariableCount;
    public int TotalDownloadedVariableDefinitionCount => _status.TotalMigratedVariableDefinitionCount;
    public int TotalFailedRecordCount => _status.TotalFailedRecordCount;
    public int TotalGroupCount { get => _status.TotalGroupCount; set { } }
    public int TotalRecordCount { get => _status.TotalRecordCount; set { } }
    public TimeSpan? TotalTime => _status.TotalTime;


    // Not supported by resonite
    public int TotalAssetCount { get => -1; set { } }
    public int TotalDownloadedAssetCount { get => -1; set { } }

    private ResoniteRecordDownloadStatus _UserRecordsStatus;
    public IRecordDownloadStatus UserRecordsStatus
    {
        get { return _UserRecordsStatus; }
        set { }
    }

    private ResoniteVariableDownloadStatus _UserVariablesStatus;
    public IVariableDownloadStatus UserVariablesStatus
    {
        get { return _UserVariablesStatus; }
        set { }
    }

    private float Percentage(int current, int total)
    {
        if (total == 0 || current == 0)
            return 0;

        return current / total;
    }

    public float PercentageContacts => Percentage(DownloadedContactCount, TotalContactCount);
    public float PercentageGroups => Percentage(DownloadedGroupCount, TotalGroupCount);
    public float PercentageRecords => Percentage(TotalRecordCount, TotalDownloadedRecordCount);

    // TODO: This is a little ugly.
    public List<IGroupDownloadStatus> GroupStatuses { get => _status.GroupStatuses.Select(s => (IGroupDownloadStatus)new ResoniteGroupDownloadStatus(s)).ToList(); set { } }

    // TODO
    // Empty list for now
    public List<IAssetFailure> AssetFailures { get => new List<IAssetFailure>(); set { } }


    public string GenerateReport()
    {
        //TODO
        return "";
    }

    // I don't think these are used so I blank them for now!
    public IGroupDownloadStatus GetGroupStatus(string ownerId, string groupName)
    {
        throw new NotImplementedException();
    }

    public void RegisterAssetFailure(IAssetFailure failure)
    {
        throw new NotImplementedException();
    }

    public void UpdateStats()
    {
        this.Refresh();
        this._UserVariablesStatus?.Refresh();
        this._UserRecordsStatus?.Refresh();
    }
}
