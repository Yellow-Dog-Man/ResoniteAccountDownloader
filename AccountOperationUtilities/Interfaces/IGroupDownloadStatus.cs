namespace AccountOperationUtilities.Interfaces;

public interface IGroupDownloadStatus
{
    /// <summary>
    /// OwnerID of the group
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// Name of the group
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// Were members already downloaded? It doesn't make sense to have progress indicator for this,
    /// because there's barely any data to go with this.
    /// </summary>
    public int DownloadedMemberCount { get; set; }

    /// <summary>
    /// Status of the downloaded records
    /// </summary>
    public IRecordDownloadStatus RecordsStatus { get; set; }

    /// <summary>
    /// Download status of the variables of the group
    /// </summary>
    public IVariableDownloadStatus VariablesStatus { get; set; }
}
