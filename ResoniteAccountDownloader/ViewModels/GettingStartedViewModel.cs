using System.Reactive;
using ReactiveUI;

namespace ResoniteAccountDownloader.ViewModels;

public class GettingStartedViewModel : ViewModelBase, IRoutableViewModel
{
    public ReactiveCommand<Unit, IRoutableViewModel> Login { get; }
    public GettingStartedViewModel(IScreen hostScreen, IViewModelFactory viewModels) : base(hostScreen, viewModels)
    {
        Login = ReactiveCommand.CreateFromObservable(() => Navigate<LoginViewModel>());
    }
}
