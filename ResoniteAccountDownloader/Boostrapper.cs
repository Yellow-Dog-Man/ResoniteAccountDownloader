using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResoniteAccountDownloader.Services;
using ResoniteAccountDownloader.ViewModels;
using Serilog;
using Serilog.Events;
using SkyFrost.Base;
using System;
using System.Diagnostics;
using System.IO;

namespace ResoniteAccountDownloader
{
    public class Config
    {
        public string LogFolder { get; }

        public LogEventLevel LogLevel { get; }

        public Config(IAssemblyInfoService info)
        {
            LogFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), info.CompanyName, info.Name);

            LogLevel = LogEventLevel.Information;
#if DEBUG
            LogLevel = LogEventLevel.Debug;
#endif

        }
    }

    public static class Boostrapper
    {
        public static ServiceProvider BuildServiceProvider()
        {
            // These are needed to configure logging, so they're created up front rather than by the container.
            var info = new AssemblyInfoService();
            var config = new Config(info);

            var services = new ServiceCollection();

            services.AddSingleton<IAssemblyInfoService>(info);
            services.AddSingleton(config);

            // Serilog does the level filtering per sink, so let everything through to it.
            services.AddLogging(builder => builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddSerilog(CreateSerilogLogger(info, config), dispose: true));

            // Registering this as non-lazy because it is quite slow to init.

            services.AddSingleton<ILocaleService, LocaleService>();
            services.AddSingleton<IAppCloudService, SkyFrostCloudService>();
            services.AddSingleton<IIdService, IdService>();
            services.AddSingleton<ContributionsService>();
            services.AddTransient<IAccountDownloader, ResoniteAccountDownloadManager>();
            services.AddTransient<IStorageService, CloudStorageService>();
            services.AddTransient<IGroupsService, GroupsService>();

            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton((sp) => {
                var idGen = sp.GetRequiredService<IIdService>();
                return new SkyFrostInterface(idGen.UID, idGen.SecretMachineId, SkyFrostConfig.DEFAULT_PRODUCTION.WithUserAgent(info.NameNoSpaces).WithoutSignalR());
            });

            return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        private static Serilog.ILogger CreateSerilogLogger(IAssemblyInfoService info, Config config)
        {
            var machine = Environment.MachineName;
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(config.LogFolder + $"/{timestamp}-{machine}-{info.Version}-.log", restrictedToMinimumLevel: config.LogLevel)
                .CreateLogger();

            Trace.Listeners.Add(new SerilogTraceListener.SerilogTraceListener(logger));

            return logger;
        }
    }
}
