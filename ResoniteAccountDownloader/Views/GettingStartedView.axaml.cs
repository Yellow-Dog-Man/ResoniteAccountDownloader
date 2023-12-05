using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ResoniteAccountDownloader.ViewModels;

namespace ResoniteAccountDownloader.Views
{
    public partial class GettingStartedView : ReactiveUserControl<GettingStartedViewModel>
    {
        public GettingStartedView()
        {
            InitializeComponent();
        }
    }
}
