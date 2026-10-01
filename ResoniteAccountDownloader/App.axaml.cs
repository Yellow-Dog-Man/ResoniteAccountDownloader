using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ResoniteAccountDownloader.ViewModels;
using ResoniteAccountDownloader.Views;

namespace ResoniteAccountDownloader
{
    public partial class App : Application
    {
        private readonly IServiceProvider? _services;

        // Used by the visual designer, which has no services.
        public App()
        {
        }

        public App(IServiceProvider services)
        {
            _services = services;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (_services != null && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindowView
                {
                    DataContext = _services.GetRequiredService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
