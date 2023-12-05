using Avalonia.ReactiveUI;
using ResoniteAccountDownloader.ViewModels;


namespace ResoniteAccountDownloader.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
    }
}

