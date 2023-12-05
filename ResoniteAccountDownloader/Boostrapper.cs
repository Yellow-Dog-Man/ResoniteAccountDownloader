using ResoniteAccountDownloader.Services;
using ResoniteAccountDownloader.ViewModels;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SkyFrost.Base;
using Splat;
using System;
using System.Diagnostics;
using System.IO;

namespace ResoniteAccountDownloader
{
    public class Config
    {
        public string LogFolder { get; }

        public LogEventLevel LogLevel { get; }

        public Config(IAssemblyInfoService? info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            LogFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), info.CompanyName, info.Name);

            LogLevel = LogEventLevel.Information;
#if DEBUG
            LogLevel = LogEventLevel.Debug;
#endif

        }
    }

    public class Boostrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolve)
        {
            services.RegisterConstant<IAssemblyInfoService>(new AssemblyInfoService());
            services.RegisterLazySingleton(() => new Config(resolve.GetService<IAssemblyInfoService>()));
            services.RegisterLazySingleton<ILogger>(() =>
            {
                var machine = Environment.MachineName;

                var info = resolve.GetService<IAssemblyInfoService>();
                var config = resolve.GetService<Config>();
                var folder = config!.LogFolder;
                var level = config!.LogLevel;

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(folder + $"/{timestamp}-{machine}-{info!.Version}-.log", restrictedToMinimumLevel: level)
                    .CreateLogger();

                var listener = new SerilogTraceListener.SerilogTraceListener(logger);
                Trace.Listeners.Add(listener);

                var factory = new SerilogLoggerFactory(logger);

                return factory.CreateLogger("Default");
            });

            services.RegisterConstant<ILocaleService>(new LocaleService(resolve.GetService<ILogger>()));

            var version = resolve.GetService<IAssemblyInfoService>();
            // Registering this as non-lazy because it is quite slow to init.
            services.RegisterConstant(new SkyFrostInterface(UID.Compute(), SkyFrostConfig.DEFAULT_PRODUCTION.WithUserAgent(version?.NameNoSpaces).WithoutSignalR()));

            services.RegisterLazySingleton<IAppCloudService>(() => new SkyFrostCloudService(resolve.GetService<SkyFrostInterface>(), resolve.GetService<ILogger>()));

            services.RegisterLazySingleton(() => new MainWindowViewModel());
            services.Register<IAccountDownloader>(() => new ResoniteAccountDownloadManager(resolve.GetService<SkyFrostInterface>(), resolve.GetService<ILogger>()));
            services.Register<IStorageService>(() => new CloudStorageService(resolve.GetService<SkyFrostInterface>(), resolve.GetService<ILogger>()));
            services.Register<IGroupsService>(() => new GroupsService(resolve.GetService<SkyFrostInterface>(), resolve.GetService<IStorageService>(), resolve.GetService<ILogger>()));
            services.RegisterLazySingleton(() => new ContributionsService(resolve.GetService<ILogger>()));
        }
    }
}
