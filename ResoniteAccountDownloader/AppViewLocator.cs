using System;
using ResoniteAccountDownloader.ViewModels;
using ReactiveUI;
using Avalonia.Controls.Templates;
using Avalonia.Controls;
using AccountOperationUtilities.Generators;
using System.Diagnostics.CodeAnalysis;

namespace ResoniteAccountDownloader;


[StaticViewLocator]
public partial class AppViewLocator : IViewLocator, IDataTemplate
{
    public AppViewLocator() {
    }

    // https://www.reactiveui.net/docs/handbook/view-location/
    public IViewFor? ResolveView<T>(T? param, string? contract = null)
    {
        var c = Build(param);
        if (c != null)
            return (IViewFor?) c;

        return null;
    }

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null) where TViewModel : class
    {
        return ResolveView<TViewModel>(contract);
    }

    [RequiresUnreferencedCode("")] //TODO: I don't know what you're meant to put here
    public IViewFor? ResolveView(object? instance, string? contract = null)
    {
        return ResolveView(instance, contract);
    }

    public bool Match(object? data) => data is ViewModelBase;

    public Control? Build(object? param)
    {
        var f = GetViewConstructor(param);
        if (f != null)
            return f.Invoke();

        throw new Exception($"Unable to create view for type: {param}");
    }

    private Func<Control>? GetViewConstructor(object? param)
    {
        if (param is null) return null;

        var type = param.GetType();
        if (StaticViews.TryGetValue(type, out var func))
        {
            return func;
        }

        return null;
    }
}
