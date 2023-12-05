using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ResoniteAccountDownloader.ViewModels;

namespace ResoniteAccountDownloader.Views;

public partial class ProgressView : ReactiveUserControl<ProgressViewModel>
{
    public ProgressView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

