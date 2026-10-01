using System;
using System.Reactive;
using ResoniteAccountDownloader.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Linq;
using ResoniteAccountDownloader.Utilities;

namespace ResoniteAccountDownloader.ViewModels;

public class LoginViewModel : ViewModelBase, IValidatableViewModel
{
    [Reactive]
    public string Username { get; set; } = string.Empty;

    [Reactive]
    public string Password { get; set; } = string.Empty;

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    public ReactiveCommand<Unit, AuthResult> Login { get; set; }

    private readonly IAppCloudService CloudService;
    public LoginViewModel(IScreen hostScreen, IViewModelFactory viewModels, IAppCloudService cloudService) : base(hostScreen, viewModels)
    {
        CloudService = cloudService;

        Login = ReactiveCommand.CreateFromTask(() => CloudService.Login(Username, Password), this.IsValid());
        Login.Subscribe(async result =>
        {
            // TOTP Required, go there.
            if (result.state == AuthenticationState.TOTPRequired)
                await Navigate<MultiFactorAuthViewModel>();
            // Authenticated, no TOTP, go to next
            else if (result.state == AuthenticationState.Authenticated)
                await Navigate<DownloadSelectionViewModel>();
            // Error, show it
            else
                await GlobalInteractions.ShowError.Handle(new MessageBoxRequest(result.error ?? Res.Errors_UnexpectedLoginError));
        });

        this.ValidationRule(viewModel => viewModel.Username, username => !string.IsNullOrWhiteSpace(username ?? null), Res.Errors_BlankUsername);
        this.ValidationRule(viewModel => viewModel.Password, password => !string.IsNullOrWhiteSpace(password ?? null), Res.Errors_BlankPassword);
    }
}
