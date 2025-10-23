using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Sustainacore.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private const string ApiKey = "AIzaSyCXjHkXlfbLAcLXnQnQBllzU5T2NffxYnA";
    private static readonly Uri Endpoint = new Uri($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}");

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter both email and password.");
                return;
            }

            var payload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var resp = await http.PostAsync(Endpoint, content);

            var respBody = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                // Try to parse Firebase error payload
                try
                {
                    var fbErr = JsonSerializer.Deserialize<FirebaseError>(respBody);
                    var msg = fbErr?.error?.message ?? resp.StatusCode.ToString();
                    ShowError($"Login failed: {msg}");
                }
                catch
                {
                    ShowError($"Login failed: {resp.StatusCode}");
                }
                return;
            }

            var auth = JsonSerializer.Deserialize<SignInResponse>(respBody);
            var idToken = auth?.idToken;
            if (string.IsNullOrWhiteSpace(idToken))
            {
                ShowError("Invalid login response.");
                return;
            }

            await SecureStorage.SetAsync("auth_token", idToken);
            await SecureStorage.SetAsync("user_email", email!);

            // Example: prepare HttpClient with Bearer for your API later
            // http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            if (Shell.Current is not null)
            {
                await Shell.Current.GoToAsync("//Dashboard");
            }
            else
            {
                await Navigation.PushAsync(new DashboardPage());
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    // --- DTOs for Firebase REST response ---
    private sealed class SignInResponse
    {
        public string idToken { get; set; } = "";
        public string email { get; set; } = "";
        public string refreshToken { get; set; } = "";
        public string expiresIn { get; set; } = "";
        public string localId { get; set; } = "";
        public bool registered { get; set; }
    }

    private sealed class FirebaseError
    {
        public ErrorDetails error { get; set; } = new();
        public sealed class ErrorDetails
        {
            public int code { get; set; }
            public string message { get; set; } = "";
        }
    }
}
