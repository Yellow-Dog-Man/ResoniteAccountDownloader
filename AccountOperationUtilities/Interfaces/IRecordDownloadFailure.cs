namespace AccountOperationUtilities.Interfaces;

public interface IRecordDownloadFailure
{
    string FailureReason { get; set; }
    string OwnerId { get; set; }
    string RecordId { get; set; }
    string RecordName { get; set; }
    string RecordPath { get; set; }
}
