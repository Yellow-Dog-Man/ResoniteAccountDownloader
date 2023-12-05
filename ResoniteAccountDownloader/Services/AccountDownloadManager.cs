

using AccountOperationUtilities.Interfaces;
using Avalonia.Threading;
using Elements.Core;
using Microsoft.Extensions.Logging;
using ResoniteAccountDownloader.Models;
using SkyFrost.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResoniteAccountDownloader.Services;
public class ResoniteAccountDownloadManager : IAccountDownloader
{
    private SkyFrostInterface Interface { get; }

    private AccountTransferController? Controller { get; set; }
    public Action<string>? ProgressMessageHandler { get; set; } = null;

    public string? DownloadPhase => Controller?.Status.Phase;

    private ResoniteDownloadStatus? _Status { get; set; } = null;
    // TODO
    public IAccountDownloadStatus? Status => _Status;

    private CancellationTokenSource? CancelTokenSource = null;

    private readonly ILogger Logger;

    private readonly DispatcherTimer StatsTimer = new();

    public ResoniteAccountDownloadManager(SkyFrostInterface? cloudInterface, ILogger? logger)
    {
        Logger = logger ?? throw new NullReferenceException("Cannot run without a logger"); ;
        Interface = cloudInterface ?? throw new NullReferenceException("Cannot run without a Resonite Account Downloader Interface");
        StatsTimer.Interval = TimeSpan.FromSeconds(1);
        StatsTimer.Tick += StatsTimer_Tick;
    }

    private void StatsTimer_Tick(object? sender, EventArgs e)
    {
        if (Controller == null)
            return;
        Controller.Status?.UpdateStats();
        _Status?.UpdateStats();
    }

    // Here we're just hiding the implementation from consumers of ResoniteAccountDownloader, it seems messy but Interfaces make this easier to develop
    // and easier to swap stuff in and out.
    private void SurfaceProgressMessage(string message)
    {
        Logger.LogInformation("Progress Message:{phase} - {message}", Controller?.Status.Phase ?? "Pending", message);

        ProgressMessageHandler?.Invoke(message);
    }

    public string LatestProgressMessage => Controller?.ProgressMessage ?? string.Empty;

    public void Cancel()
    {
        this.Logger.LogInformation("Cancelling download for {user}", Interface.CurrentUser.Username);
        if (CancelTokenSource != null && CancelTokenSource.Token.CanBeCanceled)
        {
            StatsTimer.Stop();
            CancelTokenSource.Cancel();
        }
    }

    public async Task<IDownloadResult> Start(IAccountDownloadConfig config)
    {
        this.Logger.LogInformation("Starting download for {user}", Interface.CurrentUser.Username);
        this.Logger.LogInformation("Saving output to {filePath}", config.FilePath);
        this.Logger.LogInformation("With config {config}", config);
        CancelTokenSource = new CancellationTokenSource();

        var libraryConfig = CreateConfigFromIAccountDownloadConfig(config);

        // We do not include the user's username here as Resonite Account Downloader takes care of this.
        // It'll store items owned by a user in a folder based on their User ID.
        var local = new SkyFrost.Base.LocalAccountDataStore(Interface.Platform, Interface.CurrentUser.Id, config.FilePath, config.FilePath + "/Assets");
        Controller = new SkyFrost.Base.AccountTransferController(new SkyFrost.Base.CloudAccountDataStore(Interface), local, Guid.NewGuid().ToString(), libraryConfig);
        Controller.ProgressMessagePosted += SurfaceProgressMessage;

        // Wrap the Status Model.
        _Status = new ResoniteDownloadStatus(Controller!.Status);

        StatsTimer.Start();

        var res = await StartDownload();

        // Log a status report
        if (Status != null)
            Logger.LogInformation(Status.GenerateReport());

        return res;
    }

    private async Task<IDownloadResult> StartDownload()
    {
        var result = await Controller!.Transfer(CancelTokenSource!.Token);
        this.Logger.LogInformation("Download Finished with result: {result}", result);

        StatsTimer.Stop();

        if (!result)
        {
            this.Logger.LogError(Controller?.Status?.Error);
        }

        var downloadResult = new ResoniteIDownloadResult
        {
            Result = result ? DownloadResultType.Sucessful : (CancelTokenSource.IsCancellationRequested ? DownloadResultType.Cancelled : DownloadResultType.Error)
        };

        return downloadResult;
    }

    private static AccountMigrationConfig CreateConfigFromIAccountDownloadConfig(IAccountDownloadConfig config)
    {
        var downloadConfig = new AccountMigrationConfig
        {
            MigrateCloudVariableDefinitions = config.DownloadCloudVariableDefinitions,
            MigrateCloudVariables = config.DownloadCloudVariables,
            MigrateUserProfile = config.DownloadUserMetadata,
            MigrateContacts = config.DownloadContacts,
            MigrateMessageHistory = config.DownloadMessageHistory,
            MigrateUserRecords = config.DownloadUserRecords,
            MigrateFundingEvents = config.DownloadFundingEvents,
            MigrateRecordAuditLog = config.DownloadRecordAuditLog,
            MigrateExitMessages = config.DownloadExitMessages,

            // Prevent download of groups that haven't been selected
            MigrateGroups = config.DownloadGroups.Any(),
            GroupsToMigrate = new HashSet<string>(config.GroupsToDownload),
        };

        return downloadConfig;
    }
}
