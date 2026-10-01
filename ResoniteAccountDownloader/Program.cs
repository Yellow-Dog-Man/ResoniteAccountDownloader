using Avalonia;
using System;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResoniteAccountDownloader.Services;

namespace ResoniteAccountDownloader
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) {
            // Disposing the provider on exit flushes the logs.
            using var services = Boostrapper.BuildServiceProvider();

            var logger = services.GetRequiredService<ILogger<Program>>();
            var config = services.GetRequiredService<Config>();
            var info = services.GetRequiredService<IAssemblyInfoService>();

            logger.LogInformation("Starting up {AppName} v{Version}", info.Name, info.Version);
            logger.LogInformation("Log Level: {LogLevel}", config.LogLevel);

            BuildAvaloniaApp(() => new App(services)).StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() => BuildAvaloniaApp(() => new App());

        private static AppBuilder BuildAvaloniaApp(Func<App> appFactory)
            => AppBuilder.Configure(appFactory)
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
