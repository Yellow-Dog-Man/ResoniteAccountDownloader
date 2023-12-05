using AccountOperationUtilities.Interfaces;
using Avalonia.Platform.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ResoniteAccountDownloader.Extensions;
using ResoniteAccountDownloader.Services;
using ResoniteAccountDownloader.Utilities;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ResoniteAccountDownloader.ViewModels;

public class DownloadSelectionViewModel : ViewModelBase, IAccountDownloadConfig
{
    public ReactiveCommand<Unit, Unit> OpenFolder { get; }
    public Interaction<FolderPickerOpenOptions, InteractionResult<Uri>> ShowOpenFolderDialog { get; }
    private readonly IAppCloudService CloudService;
    private readonly IGroupsService GroupService;
    private readonly IStorageService StorageService;
    public UserProfileViewModel ProfileViewModel { get; }
    public GroupsListViewModel GroupsList { get; }

    public ReactiveCommand<Unit, Unit> StartDownload { get; }

    // IAccountDownloadConfig stuff
    // Is bound from the UI to select what data to download
    [Reactive]
    public bool DownloadUserMetadata { get; set; } = false;

    [Reactive]
    public bool DownloadContacts { get; set; } = false;

    private bool PreviousMessageHistory = false;
    [Reactive]
    public bool DownloadMessageHistory { get; set; } = false;

    [Reactive]
    public bool DownloadUserRecords { get; set; } = false;

    [Reactive]
    public bool OnlyNewRecords { get; set; } = false;

    [Reactive]
    public bool DownloadCloudVariableDefinitions { get; set; } = false;

    [Reactive]
    public bool DownloadCloudVariables { get; set; } = false;

    [Reactive]
    public bool ForceOverwrite { get; set; } = false;

    public IEnumerable<string> VariablesToDownload { get; } = new HashSet<string>();

    public IEnumerable<string> GroupsToDownload { get; } = new HashSet<string>();
    public IEnumerable<string> RecordsToDownload { get; } = new HashSet<string>();

    [Reactive]
    public string FilePath { get; set; } = "";

    [ObservableAsProperty]
    public long RequiredBytes { get; } = 0;

    [ObservableAsProperty]
    public bool ShouldShowRequiredBytes { get; } = false;

    [Reactive]
    public int MaxDegreeOfParallelism { get; set; } = 8;

    public IStorageRecord UserStorage { get; }

    public IEnumerable<string> Groups => GroupsList.GetSelectedGroupIds();

    public bool DownloadGroups {
        get => Groups.Count() > 0;
        set { }
    }

    [Reactive]
    public bool DownloadFundingEvents { get; set; } = false;

    [Reactive]
    public bool DownloadRecordAuditLog { get; set; } = false;

    [Reactive]
    public bool DownloadExitMessages { get; set; } = false;

    public DownloadSelectionViewModel()
    {
        // Most of these will never trigger as our config is static, but Nullables make us do this and I kinda like how it'll point to a config issue.
        CloudService = Locator.Current.GetService<IAppCloudService>() ?? throw new NullReferenceException("Cannot download without an app service");
        GroupService = Locator.Current.GetService<IGroupsService>() ?? throw new NullReferenceException("Cannot download without a group service");
        StorageService = Locator.Current.GetService<IStorageService>() ?? throw new NullReferenceException("Cannot download without a storage service");

        // The OpenFile command is bound to a button/menu item in the UI.
        OpenFolder = ReactiveCommand.CreateFromTask(PickDownloadFolder);

        // The ShowOpenFileDialog interaction requests the UI to show the file open dialog.
        ShowOpenFolderDialog = new Interaction<FolderPickerOpenOptions, InteractionResult<Uri>>();

        ProfileViewModel = new UserProfileViewModel(CloudService.Profile);
        GroupsList = new GroupsListViewModel();
        UserStorage = StorageService.GetUserStorage();

        // TODO: yeah I agree
        // This is kinda gross
        var hasDownloadSelection = this.WhenAnyValue(
            x => x.DownloadUserMetadata,
            x => x.DownloadContacts,
            x => x.DownloadMessageHistory,
            x => x.DownloadUserRecords,
            x => x.DownloadCloudVariableDefinitions,
            x => x.DownloadCloudVariables,
            x => x.DownloadFundingEvents,
            x => x.DownloadRecordAuditLog,
            x => x.DownloadExitMessages,
            (userMetadata, contacts, messagehistory, inventoryWorlds, cloudVariableDefinitions,
                cloudVariableValues, recordAuditLog, fundingEvents, exitMessages) => userMetadata || contacts || messagehistory
                    || inventoryWorlds || cloudVariableDefinitions || cloudVariableValues || recordAuditLog || fundingEvents || exitMessages
        );
        var hasFilePath = this.WhenAnyValue(x => x.FilePath, filePath => !string.IsNullOrEmpty(filePath));

        var canDownload = hasDownloadSelection.Zip(hasFilePath, (selection, filepath) => selection && filepath);

        StartDownload = ReactiveCommand.CreateFromTask(() => StartDownloadFn(), canDownload);

        // When Contacts is changed, disable message history, but preserve the previous value
        this.WhenAnyValue(x => x.DownloadContacts).Subscribe(x =>
        {
            if (!x)
            {
                PreviousMessageHistory = DownloadMessageHistory;
                DownloadMessageHistory = false;
            }
            else
            {
                DownloadMessageHistory = PreviousMessageHistory;
            }
        });

        // When either the inventory worlds checkbox changes or the requiredbytes propert of our group list changes, update the bytes total
        // This too can probably be improved.
        var byteTotal = this.
            WhenAnyValue(x => x.DownloadUserRecords, x => x.GroupsList.RequiredBytes)
            .Select((p) =>
            {
                var bytes = p.Item1 ? UserStorage.UsedBytes : 0;
                bytes += p.Item2;
                return bytes;
            });

        // Leave the bytes as a number
        byteTotal.ToPropertyEx(this, x => x.RequiredBytes);

        // bytes > 0 => true
        byteTotal.Select(x => x > 0).ToPropertyEx(this, x => x.ShouldShowRequiredBytes);

        //TODO: errors
        _ = LoadGroups();
    }

    private async Task LoadGroups()
    {
        var groups = await GroupService.GetGroups();
        GroupsList.AddGroups(groups);
    }

    private async Task StartDownloadFn()
    {
        await Router.Navigate.Execute(new ProgressViewModel(this));
    }

    private async Task PickDownloadFolder()
    {
        var fileName = await ShowOpenFolderDialog.Handle(new FolderPickerOpenOptions { AllowMultiple = false, Title = Res.WhereDownload });

        if (!fileName.HasResult)
        {
            await GlobalInteractions.ShowMessageBox.Handle(new MessageBoxRequest(fileName.Error!));
            return;
        }

        // Suffix the returned path with ResoniteDownload, this ensures we send stuff to its own folder
        // Before we had this, it was possible for some users to accidentally dump the Resonite Download into their Documents folder.
        // This is due to the Download using multiple folders for each user or the assets downloaded.
        // So if a user accidentally selects Documents, for example, then:
        // Documents\U-User & Documents\Assets would be created.

        // That could be confusing, so we suffix our own folder to it which would mean:
        // Documents\ResoniteDownload\U-User & Documents\ResoniteDownload\Assets would be created
        // This keeps the files in one folder no matter what people do

        // It could lead to some confusion if people already understand this process and create their own folder for it
        // E.g. Documents\ResoniteBackup(user made)\ResoniteDownload(we made this).
        // But user's can always edit the path after selection

        // So anyway, suffix the returned path with ResoniteDownload
        // See: https://stackoverflow.com/questions/372865/path-combine-for-urls
        // This is so complicated why?
        var downloadAppend = "ResoniteDownload";

        var uri = fileName.Result;

        // Check if we're in an existing Download location.
        // Fixes #33
        if (!IsDownloadLocation(uri!.LocalPath))
        {
            if (!uri!.AbsoluteUri.EndsWith(downloadAppend))
                uri = uri.Append("/" + downloadAppend);
        }

        FilePath = uri.LocalPath;
    }

    //TODO: IO Service
    //TODO: We should dump a "Marker" file in the directory detailing the version that wrote the local data store.
    public bool IsDownloadLocation(string path)
    {
        DirectoryInfo location = new DirectoryInfo(path);
        if (!location.Exists)
            return false;

        // Does the location already contain an Assets folder which signifies that it is already a download location.
        return location.GetDirectories().Any(d => d.Name == "Assets");
    }

    public override string ToString()
    {
        return $"Contacts:{DownloadContacts}, MessageHistory:{DownloadMessageHistory}, InventoryWorlds:{DownloadUserRecords}, CloudVariableDefinitions:{DownloadCloudVariableDefinitions}, CloudVariableValues:{DownloadCloudVariables}, Groups:{GroupsList}";
    }
}
