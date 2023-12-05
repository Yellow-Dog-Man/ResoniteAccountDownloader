using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ResoniteAccountDownloader.ViewModels;
using ReactiveUI;

namespace ResoniteAccountDownloader.Views;

public partial class MultiFactorAuthView : ReactiveUserControl<MultiFactorAuthViewModel>
{
    public MultiFactorAuthView()
    {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.OTPBox?.Focus();
        });
    }
}

