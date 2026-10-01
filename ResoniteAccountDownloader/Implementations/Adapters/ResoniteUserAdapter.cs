using AccountOperationUtilities.Interfaces;
using System;
using SkyFrost.Base;
using System.ComponentModel;

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
        if (User.Profile == null)
            return null;

        Uri uri;

        if (Uri.TryCreate(User.Profile.IconUrl, UriKind.Absolute, out uri!))
            return Interface.Assets.DBToHttp(uri, DB_Endpoint.Default);
        else
            return null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static ResoniteUserAdapter FromResoniteUser(User user, SkyFrostInterface cloudInterface)
    {
        return new ResoniteUserAdapter(user, cloudInterface);
    }
}
