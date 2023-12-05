using SkyFrost.Base;

namespace ResoniteAccountDownloader.Models.Adapters;

public class ResoniteRecordAdapater : AccountOperationUtilities.Interfaces.IRecord
{
    private Record Record { get; set; }

    public string Name => Record.Name;

    public string OwnerId => Record.OwnerId;

    public string Path => Record.Path;

    public ResoniteRecordAdapater(Record record)
    {
        Record = record;
    }

    public static ResoniteRecordAdapater FromResoniteRecord(Record record)
    {
        return new ResoniteRecordAdapater(record);
    }
}
