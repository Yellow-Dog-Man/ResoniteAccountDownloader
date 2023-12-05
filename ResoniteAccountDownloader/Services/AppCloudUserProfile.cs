

using System;
using ResoniteAccountDownloader.Utilities;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using AccountOperationUtilities.Interfaces;

namespace ResoniteAccountDownloader.Services;

public class AppCloudUserProfile : ReactiveObject, IUserProfile
{

    [Reactive]
    public string UserName { get; set; } = "Unauthenticated";

    [Reactive]
    public Uri PictureURI { get; set; } = AssetHelper.GetUri("AnonymousHeadset.png");

    private static ILogger? Logger;

    public AppCloudUserProfile(IUser? user = null)
    {
        if (user != null)
            UpdateUser(user);

        Logger = Locator.Current.GetService<ILogger>() ?? throw new NullReferenceException("No logger found");
    }

    public void UpdateUser(IUser? user)
    {
        Logger?.LogDebug($"Updating user New:{user?.Username}, Old:{UserName} ");
        if (user == null)
            return;

        UserName = user.Username;

        //TODO: move to Resonite Adapter
        ////GetProfilePictureUri(user?.Profile?.IconUrl) 
        PictureURI = user?.PictureURI ?? AssetHelper.GetUri("AnonymousHeadset.png");
    }
}
