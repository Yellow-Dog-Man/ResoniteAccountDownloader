using AccountOperationUtilities.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ResoniteAccountDownloader.ViewModels
{
	public class ProgressStatisticsViewModel : ReactiveObject
	{
        [Reactive]
        public IAccountDownloadConfig Config { get; private set; }
        [Reactive]
        public IAccountDownloadStatus Status { get; private set; }

        public ProgressStatisticsViewModel(IAccountDownloadConfig config, IAccountDownloadStatus status)
        {
            Config = config;
            Status = status;
        }
    }
}
