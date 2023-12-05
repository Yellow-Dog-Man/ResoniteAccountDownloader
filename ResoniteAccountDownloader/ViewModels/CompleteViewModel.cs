﻿using AccountOperationUtilities.Interfaces;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ResoniteAccountDownloader.Utilities;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ResoniteAccountDownloader.ViewModels;

// This is just to give us another page to look at.
public class CompleteViewModel : ViewModelBase
{
    public IAccountDownloadStatus Status { get; }
    public IAccountDownloadConfig Config { get; }

    public ProgressStatisticsViewModel ProgressStatistics { get; }

    public FailedRecordsViewModel FailedRecords { get; }

    [Reactive]
    public bool ShouldShowFailureMessage { get; set; } = false;

    public ReactiveCommand<Unit, IRoutableViewModel> StartAnotherDownload { get; }
    public ReactiveCommand<Unit, Unit> OpenDownloadFolder { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }
    public CompleteViewModel(IAccountDownloadConfig config, IAccountDownloadStatus status)
    {
        Status = status;
        Config = config;
        ProgressStatistics = new ProgressStatisticsViewModel(config, status);

        // Zip up all the records that have failed
        List<IRecordDownloadFailure> list = new(status.UserRecordsStatus.FailedRecords);
        foreach (var g in Status.GroupStatuses)
        {
            list.AddRange(g.RecordsStatus.FailedRecords);
        }

        FailedRecords = new FailedRecordsViewModel(list, Status.AssetFailures);

        if (list.Count > 0 || Status.AssetFailures.Count > 0)
            ShouldShowFailureMessage = true;

        StartAnotherDownload = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new DownloadSelectionViewModel()));
        OpenDownloadFolder = ReactiveCommand.CreateFromTask(() => OpenDownloadFolderFn());

        Exit = ReactiveCommand.Create(ExitFn);

    }

    private void ExitFn()
    {
        //TODO: Test on more exotic os
        //TODO: should be on view layer
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    public async Task OpenDownloadFolderFn()
    {
        await GlobalInteractions.OpenFolderLocation.Handle(Config.FilePath);
    }
}
