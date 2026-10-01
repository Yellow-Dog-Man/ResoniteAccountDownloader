using SkyFrost.Base;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace ResoniteAccountDownloader.Services;
// We were having login issues, it was caused by use not using the same ID everywhere, this fixes that.
public interface IIdService
{
    public string CreateId();
    public string SecretMachineId { get; }

    public string UID { get; }
}

public class IdService : IIdService
{
    private Lazy<string> _secretMachineId = new Lazy<string>(() => CreateIdStatic());
    public string SecretMachineId => _secretMachineId.Value;

    private Lazy<string> _uid = new Lazy<string>(() => SkyFrost.Base.UID.Compute());

    public string UID => _uid.Value;

    public static string CreateIdStatic()
    {
        return Guid.CreateVersion7().ToString();
    }

    public string CreateId()
    {
        return IdService.CreateIdStatic();
    }
}
