using System.Net.Http.Json;
using System.Text.Json;

public class FirebaseAuthService
{
    private readonly IHttpClientFactory _http;
    private readonly FirebaseConfig _cfg;

    public FirebaseAuthService(IHttpClientFactory http, FirebaseConfig cfg)
    {
        _http = http;
        _cfg = cfg;
    }

    public async Task<(bool ok, string message, string? idToken, string? displayName)> RegisterAsync(string email, string password, string displayName)
    {
        var client = _http.CreateClient("FirebaseAuth");
        var payload = new
        {
            email,
            password,
            displayName,
            returnSecureToken = true
        };
        var resp = await client.PostAsJsonAsync($"accounts:signUp?key={_cfg.ApiKey}", payload);
        if (!resp.IsSuccessStatusCode)
            return (false, await resp.Content.ReadAsStringAsync(), null, null);

        var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()).RootElement;
        var idToken = doc.GetProperty("idToken").GetString();
        return (true, "OK", idToken, displayName);
    }

    public async Task<(bool ok, string message, string? idToken, string? displayName)> LoginAsync(string email, string password)
    {
        var client = _http.CreateClient("FirebaseAuth");
        var payload = new { email, password, returnSecureToken = true };
        var resp = await client.PostAsJsonAsync($"accounts:signInWithPassword?key={_cfg.ApiKey}", payload);
        if (!resp.IsSuccessStatusCode)
            return (false, await resp.Content.ReadAsStringAsync(), null, null);

        var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync()).RootElement;
        var idToken = json.GetProperty("idToken").GetString();
        var displayName = json.TryGetProperty("displayName", out var dn) ? dn.GetString() : email;
        return (true, "OK", idToken, displayName);
    }
}
