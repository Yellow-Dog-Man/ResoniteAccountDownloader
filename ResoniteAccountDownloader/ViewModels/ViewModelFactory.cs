using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ResoniteAccountDownloader.ViewModels;

/// <summary>
/// Creates view models, resolving their services from the container and filling the
/// remaining constructor parameters (e.g. the host screen or navigation state) from <paramref name="args"/>.
/// </summary>
public interface IViewModelFactory
{
    T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] args) where T : class;
}

public class ViewModelFactory(IServiceProvider services) : IViewModelFactory
{
    public T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] args) where T : class
        => ActivatorUtilities.CreateInstance<T>(services, args);
}
