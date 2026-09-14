using System.Net.Http.Json;

namespace MMO.Client.Network;

public sealed record SessionToken(string Token, DateTime CreatedAt, DateTime ExpiresAt);

public sealed record LoginResponse(SessionToken SessionToken);

public sealed record LoginRequest(string Username, string Password);

public sealed record CreateAccountRequest(string Username, string Password);

public class AuthClient
{
    private readonly HttpClient _http;

    public AuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<SessionToken?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/login", new LoginRequest(username, password), ct);
            if (!resp.IsSuccessStatusCode)
                return null;
            var payload = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
            return payload?.SessionToken;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public async Task<bool> RegisterAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/create", new CreateAccountRequest(username, password), ct);
            return resp.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}
