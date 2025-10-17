using System.Net.Http.Json;
using System.Text.Json;

public class FirebaseAuthClient
{
    private readonly HttpClient _http = new();
    private readonly string _apiKey;
    private readonly string _authBase;

    public FirebaseAuthClient()
    {
        using var s = File.OpenRead(Path.Combine(FileSystem.AppDataDirectory, "appsettings.json"));
        var cfg = JsonDocument.Parse(s).RootElement;
        _apiKey = cfg.GetProperty("Firebase").GetProperty("ApiKey").GetString()!;
        _authBase = cfg.GetProperty("Firebase").GetProperty("AuthBase").GetString()!;
        _http.BaseAddress = new Uri(_authBase);
    }

    public async Task<(bool ok, string message, string? idToken, string? displayName)> LoginAsync(string email, string password)
    {
        var payload = new { email, password, returnSecureToken = true };
        var resp = await _http.PostAsJsonAsync($"accounts:signInWithPassword?key={_apiKey}", payload);
        if (!resp.IsSuccessStatusCode) return (false, await resp.Content.ReadAsStringAsync(), null, null);
        var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()).RootElement;
        return (true, "OK", json.GetProperty("idToken").GetString(), json.TryGetProperty("displayName", out var dn) ? dn.GetString() : email);
    }

    public async Task<(bool ok, string message, string? idToken, string? displayName)> RegisterAsync(string email, string password, string displayName)
    {
        var payload = new { email, password, displayName, returnSecureToken = true };
        var resp = await _http.PostAsJsonAsync($"accounts:signUp?key={_apiKey}", payload);
        if (!resp.IsSuccessStatusCode) return (false, await resp.Content.ReadAsStringAsync(), null, null);
        var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()).RootElement;
        return (true, "OK", json.GetProperty("idToken").GetString(), displayName);
    }
}
