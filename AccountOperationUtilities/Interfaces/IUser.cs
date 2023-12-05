namespace AccountOperationUtilities.Interfaces;

/// <summary>
/// Represents any generic user.
/// </summary>
// We use this to wrap our User models so that multiple endpoints can be used
public interface IUser
{
    public string Id { get; }
    public string Username { get; }
    public Uri? PictureURI { get; }

    public long UsedBytes { get; }
    public long QuotaBytes { get; }
}
