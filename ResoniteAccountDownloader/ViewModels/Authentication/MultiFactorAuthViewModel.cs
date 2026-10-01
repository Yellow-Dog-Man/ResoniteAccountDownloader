using System;
using System.Reactive;
using ResoniteAccountDownloader.Services;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System.Reactive.Linq;
using ResoniteAccountDownloader.Utilities;

namespace ResoniteAccountDownloader.ViewModels;

public class MultiFactorAuthViewModel : ViewModelBase, IValidatableViewModel
{
    [Reactive]
    public string? TOTPToken { get; set; } = "";

    private IAppCloudService CloudService;

    public ReactiveCommand<Unit, AuthResult> SubmitTOTP { get; set; }

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    public Interaction<string?, Unit> ShowError { get; }

    public MultiFactorAuthViewModel(IScreen hostScreen, IViewModelFactory viewModels, IAppCloudService cloudService) : base(hostScreen, viewModels)
    {
        CloudService = cloudService;

        ShowError = new Interaction<string?, Unit>();

        SubmitTOTP = ReactiveCommand.CreateFromTask(() => CloudService.SubmitTOTP(TOTPToken), this.IsValid());

        this.ValidationRule(viewModel => viewModel.TOTPToken, token => !string.IsNullOrWhiteSpace(token ?? null), "TOTP Token cannot be blank");

        SubmitTOTP.Subscribe(async result =>
        {
            if (result.state == AuthenticationState.TOTPRequired)
            {
                await GlobalInteractions.ShowError.Handle(new MessageBoxRequest("Invalid TOTP Token."));
                return;
            }
            if (result.state == AuthenticationState.Authenticated)
            {
                await Navigate<DownloadSelectionViewModel>();
                return;
            }

            await GlobalInteractions.ShowError.Handle(new MessageBoxRequest(result.error ?? "An unknown error occurred when entering TOTP token."));

            // We have to return back to the login screen as regular username and password credentials are not checked until after the TOTP
            // token is entered correctly.
            await Navigate<LoginViewModel>();
        });
    }
}
