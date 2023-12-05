using ResoniteAccountDownloader.Services;
using ReactiveUI;

namespace ResoniteAccountDownloader.ViewModels
{
    public class UserProfileViewModel : ReactiveObject
    {
        public IUserProfile Profile { get; set; }

        public UserProfileViewModel(IUserProfile profile)
        {
            Profile = profile;
        }
    }
}
