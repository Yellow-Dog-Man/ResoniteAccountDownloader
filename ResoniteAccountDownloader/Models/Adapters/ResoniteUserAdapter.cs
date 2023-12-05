using AccountOperationUtilities.Interfaces;
using System;
using SkyFrost.Base;
using System.ComponentModel;
using Splat;

namespace ResoniteAccountDownloader.Models.Adapters;

// Wraps a Resonite User into an IUser interface. This avoids binding any uses of User to a single platform
public class ResoniteUserAdapter : IUser
{
    private User User;
    private SkyFrostInterface Interface;

    public ResoniteUserAdapter(User user, SkyFrostInterface _interface)
    {
        User = user;
        Interface = _interface;
    }

    public string Username => User.Username;

    public Uri? PictureURI => GetProfilePicture();

    // TODO: These are obsolete
    public long UsedBytes => User.LegacyUsedBytes ?? 0;
    public long QuotaBytes => User.LegacyQuotaBytes ?? 0;

    public string Id => User.Id;

    private Uri? GetProfilePicture()
    {
        Uri uri;
        var success = Uri.TryCreate(User.Profile.IconUrl, UriKind.Absolute, out uri!);

        if (!success)
            return null;

        return Interface.Assets.DBToHttp(uri, DB_Endpoint.Default);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static ResoniteUserAdapter FromResoniteUser(User user)
    {
        SkyFrostInterface i = Locator.Current.GetService<SkyFrostInterface>() ?? throw new NullReferenceException("Cannot run without a SkyFrost Interface");

        return new ResoniteUserAdapter(user, i);
    }
}
