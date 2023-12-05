namespace AccountOperationUtilities.Interfaces;

public interface IVariableDownloadStatus
{
    public int DownloadedVariableCount { get; set; }
    public int DownloadedVariableDefinitionCount { get; set; }
    public string GenerateReport();
}
