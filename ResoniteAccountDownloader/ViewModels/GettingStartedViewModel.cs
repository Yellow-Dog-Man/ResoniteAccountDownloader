using System.Reactive;
using ReactiveUI;

namespace ResoniteAccountDownloader.ViewModels;

public class GettingStartedViewModel : ViewModelBase, IRoutableViewModel
{
    public ReactiveCommand<Unit, IRoutableViewModel> Login { get; }
    public GettingStartedViewModel()
    {
        Login = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new LoginViewModel()));
    }
}
