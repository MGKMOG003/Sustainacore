using System.IdentityModel.Tokens.Jwt;

namespace Sustainacore.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseAuthClient _auth;

    public LoginPage()
    {
        InitializeComponent();
        _auth = new FirebaseAuthClient();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var (ok, msg, token, display) = await _auth.LoginAsync(EmailEntry.Text!, PasswordEntry.Text!);
        if (!ok || string.IsNullOrEmpty(token)) { StatusLabel.Text = "Login failed."; return; }

        var role = DecodeRole(token) ?? "Client";
        await NavigateToDashboard(role, display ?? EmailEntry.Text!);
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var (ok, msg, token, display) = await _auth.RegisterAsync(EmailEntry.Text!, PasswordEntry.Text!, EmailEntry.Text!);
        if (!ok || string.IsNullOrEmpty(token)) { StatusLabel.Text = "Registration failed."; return; }

        await NavigateToDashboard("Client", display ?? EmailEntry.Text!);
    }

    private static string? DecodeRole(string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        return token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
    }

    private Task NavigateToDashboard(string role, string displayName)
    {
        Page target = role switch
        {
            "Admin" => new Dashboards.AdminDashboard(displayName),
            "ProjectManager" => new Dashboards.PMDashboard(displayName),
            "Contractor" => new Dashboards.ContractorDashboard(displayName),
            _ => new Dashboards.ClientDashboard(displayName)
        };
        Application.Current!.MainPage = new NavigationPage(target);
        return Task.CompletedTask;
    }
}

