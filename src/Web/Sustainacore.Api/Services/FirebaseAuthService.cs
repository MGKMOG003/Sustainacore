using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sustainacore.Api.Services
{
    public sealed class FirebaseAuthService
    {
        private readonly string _apiKey;
        private readonly string _authBase;
        private readonly HttpClient _http;

        public FirebaseAuthService(IConfiguration config, IHttpClientFactory httpFactory)
        {
            _apiKey  = config["Firebase:ApiKey"]  ?? throw new InvalidOperationException("Missing Firebase:ApiKey");
            _authBase = config["Firebase:AuthBase"] ?? "https://identitytoolkit.googleapis.com/v1";
            _http = httpFactory.CreateClient(nameof(FirebaseAuthService));
        }

        public async Task<(bool ok, string idToken, string? error)> SignInAsync(string email, string password)
        {
            var url = $"{_authBase}/accounts:signInWithPassword?key={_apiKey}";
            var payload = new { email, password, returnSecureToken = true };
            using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var resp = await _http.PostAsync(url, content);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                try
                {
                    var err = JsonSerializer.Deserialize<FirebaseError>(body);
                    return (false, string.Empty, err?.error?.message ?? resp.StatusCode.ToString());
                }
                catch
                {
                    return (false, string.Empty, resp.StatusCode.ToString());
                }
            }

            var ok = JsonSerializer.Deserialize<SignInResponse>(body);
            return (true, ok?.idToken ?? string.Empty, null);
        }

        private sealed class SignInResponse
        {
            public string idToken { get; set; } = string.Empty;
        }

        private sealed class FirebaseError
        {
            public ErrorDetails error { get; set; } = new();
            public sealed class ErrorDetails
            {
                public string message { get; set; } = string.Empty;
            }
        }
    }
}
