using AccountOperationUtilities.Interfaces;
using SkyFrost.Base;
using System.ComponentModel;

namespace ResoniteAccountDownloader.Models.Adapters
{
    public class ResoniteVariableDownloadStatus : RefreshableModel, IVariableDownloadStatus
    {
        private VariableMigrationStatus _status;
        public ResoniteVariableDownloadStatus(VariableMigrationStatus status)
        {
            _status = status;
        }

        public int DownloadedVariableCount { get => _status.MigratedVariableCount; set { } }
        public int DownloadedVariableDefinitionCount { get => _status.MigratedVariableDefinitionCount; set { } }


        public string GenerateReport()
        {
            return "";
        }
    }
}
