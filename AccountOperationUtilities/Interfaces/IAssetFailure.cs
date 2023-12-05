namespace AccountOperationUtilities.Interfaces;

public interface IAssetFailure
{
    IRecord? ForRecord { get; }
    string Hash { get; }
    string OwnerId { get; }
    string Reason { get; }
    string RecordName { get; }
    string RecordPath { get; }
}
