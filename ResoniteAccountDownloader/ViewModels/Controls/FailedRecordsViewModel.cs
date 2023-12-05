using System.Collections.Generic;
using System.Collections.ObjectModel;
using AccountOperationUtilities.Interfaces;
using ReactiveUI;

namespace ResoniteAccountDownloader.ViewModels;

// TODO: Collate all failures into a single model, I hated duplicating the work here.
public class FailedRecordsViewModel : ReactiveObject
{
    public ObservableCollection<IRecordDownloadFailure> FailedRecords {get;}
    public ObservableCollection<IAssetFailure> FailedAssets { get; }
    public bool ShouldShowRecordFailures { get; }
    public bool ShouldShowAssetFailures { get; }

    public FailedRecordsViewModel(List<IRecordDownloadFailure> recordFailures, List<IAssetFailure> assetFailures)
    {
        FailedRecords = new ObservableCollection<IRecordDownloadFailure>(recordFailures);
        FailedAssets = new ObservableCollection<IAssetFailure>(assetFailures);
        ShouldShowRecordFailures = FailedRecords.Count > 0;
        ShouldShowAssetFailures = FailedAssets.Count > 0;
    }
}
