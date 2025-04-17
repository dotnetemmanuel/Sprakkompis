using System.Diagnostics;

namespace Sprakkompis.Web.Features.Identity.Login;

public class LoginService(HttpClient _httpClient)
{
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/identity/login", new
            {
                email,
                password
            });

            if (response.IsSuccessStatusCode)
            {
                return new LoginResult(true);
            }

            var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return new LoginResult(false)
            {
                Errors = errorContent?.Errors ?? new[] { "Login failed" }
            };
        }
        catch (Exception ex)
        {
            return new LoginResult(false)
            {
                Errors = new[] { ex.Message }
            };
        }
    }

    private record ErrorResponse(string[] Errors);
}

public record LoginResult(bool Success)
{
    public string[] Errors { get; set; } = Array.Empty<string>();
}
