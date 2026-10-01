using ReactiveUI;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ResoniteAccountDownloader.ViewModels;

public class ViewModelBase : ReactiveObject, IRoutableViewModel
{
    // Reference to IScreen that owns the routable view model.
    public IScreen HostScreen { get; }

    public RoutingState Router { get; }

    protected IViewModelFactory ViewModels { get; }

    // Unique identifier for the routable view model.
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    public ViewModelBase(IScreen hostScreen, IViewModelFactory viewModels)
    {
        HostScreen = hostScreen;
        Router = hostScreen.Router;
        ViewModels = viewModels;
    }

    /// <summary>
    /// Creates a view model hosted on the same screen and navigates to it.
    /// </summary>
    /// <param name="args">Constructor arguments that don't come from the container.</param>
    protected IObservable<IRoutableViewModel> Navigate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] args) where T : ViewModelBase
        => Router.Navigate.Execute(ViewModels.Create<T>([HostScreen, .. args]));
}
