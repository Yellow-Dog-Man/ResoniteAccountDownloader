using System.ComponentModel;

namespace ResoniteAccountDownloader.Models;

// Why does this exist?
// It exists because most SkyFrost Models, don't implement INotifyPropertyChanged.
// Avalonia requires this: https://docs.avaloniaui.net/docs/guides/data-binding/inotifypropertychanged
// In order to update UIs, as we don't support that here, we'll have to wrap it.
// We're also using interfaces so we can adapt between various platforms, without re-writing all the Avalonia stuff.
public abstract class RefreshableModel : INotifyPropertyChanged
{
    // Calling this with PropertyChangedEventArgs(null), means "All properties have changed"
    // https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=net-8.0#remarks
    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}
