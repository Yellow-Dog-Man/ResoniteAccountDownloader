using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ResoniteAccountDownloader.Models.Adapters;
using SkyFrost.Base;

namespace ResoniteAccountDownloader.Services;
public class SkyFrostCloudService : IAppCloudService
{
    private string _login = string.Empty;
    private string _password = string.Empty;
    private readonly string _id;


    private readonly SkyFrostInterface _interface;
    private readonly ILogger logger;

    public SkyFrostCloudService(SkyFrostInterface? cloudInterface, ILogger? logger)
    {
        _id = Guid.NewGuid().ToString();
        _interface = cloudInterface ?? throw new NullReferenceException("Cannot run without a Resonite Account Downloader Interface");
        this.logger = logger ?? throw new NullReferenceException("Cannot run without a Logger");

        _interface.Session.UserUpdated += OnUserUpdated;

    }

    private void OnUserUpdated(User obj)
    {
        // Wrap it to an IUser
    }

    public AuthenticationState AuthState { get; private set; }

    public IUserProfile Profile { get; private set; } = new AppCloudUserProfile();

    public User User { get => _interface.CurrentUser; }

    public async Task<AuthResult> Login(string login, string password)
    {
        this.logger.LogInformation("Logging in user: {user}", login);
        var loginResult = await _interface.Session.Login(login, new PasswordLogin() { Password=password}, _id, false, null).ConfigureAwait(false);
        _login = login;
        _password = password;
        return ProcessLoginResult(loginResult);
    }

    public async Task<AuthResult> Logout()
    {
        this.logger.LogInformation("Logging out user: {user}", _login);
        _login = string.Empty;
        _password = string.Empty;

        await _interface.Session.Logout(true);

        AuthState = AuthenticationState.Unauthenticated;
        return new AuthResult(AuthenticationState.Unauthenticated, null);
    }

    private void PostLogin()
    {
        // Force an update to the user's information, this will get user profile data and capitalization correct
        // This get's called when the interface logs in but we have no real way to know when that happens. As such we'll do it again.
        //await _interface.UpdateCurrentUserInfo();

        // Flash the profile with the new data
        Profile.UpdateUser(ResoniteUserAdapter.FromResoniteUser(_interface.CurrentUser));
    }

    private AuthResult ProcessLoginResult(CloudResult<UserSessionResult<UserSession>>? loginResult)
    {
        AuthResult authResult;
        if (loginResult == null)
        {
            this.logger.LogWarning("Failed to login {user}, with reason {reason}", _login, "Resonite Account Downloader returned null from a login request");
            authResult = new AuthResult(AuthenticationState.Error, "Resonite Account Downloader returned null from a login request");
        } else
        {
            if (loginResult.IsOK)
            {
                authResult = new AuthResult(AuthenticationState.Authenticated, null);
                this.logger.LogInformation("{user} Logged in successfully.", _login);
                PostLogin();
            }
            else
            {
                if (loginResult.Content == "TOTP")
                {
                    this.logger.LogInformation("TOTP Challenge");
                    authResult = new AuthResult(AuthenticationState.TOTPRequired, "TOTP Required");
                }
                else
                {
                    this.logger.LogWarning("Failed to login {user}, with reason {reason}", _login, loginResult.Content);
                    authResult = new AuthResult(AuthenticationState.Error, loginResult.Content);
                }

            }
        }

        AuthState = authResult.state;
        return authResult;
    }

    public async Task<AuthResult> SubmitTOTP(string code)
    {
        this.logger.LogInformation("{user} responded to TOTP Challenge",_login);
        var loginResult = await _interface.Session.Login(_login, new PasswordLogin() { Password = _password }, _id, false, code).ConfigureAwait(false);
        this.logger.LogDebug("Returned from TOTP Login");
        return ProcessLoginResult(loginResult);
    }
}
