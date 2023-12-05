using System.Collections.Generic;
using System.Text.Json;

using Avalonia.Platform;

using ResoniteAccountDownloader.Utilities;
using ResoniteAccountDownloader.Models;
using System;
using Microsoft.Extensions.Logging;

namespace ResoniteAccountDownloader.Services
{

    public class ContributionsService
    {
        public List<Contributor>? Contributors => file?.Contributors;
        private readonly ContributorsFile? file;

        public ContributionsService(ILogger? logger)
        {
            try
            {
                file = JsonSerializer.Deserialize(AssetLoader.Open(AssetHelper.GetUri(".all-contributorsrc")), SourceGenerationContext.Default.ContributorsFile);
            }
            catch(Exception e)
            {
                logger?.LogError("Failed to load contributor information due to: {message}", e.Message);
            }
        }
    }
}
