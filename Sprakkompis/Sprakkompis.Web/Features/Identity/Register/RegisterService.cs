using Sprakkompis.Web.Features.Identity.Login;

namespace Sprakkompis.Web.Features.Identity.Register;

public class RegisterService(HttpClient _httpclient, LoginService _loginService)
{
    public async Task<RegisterResult> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await _httpclient.PostAsJsonAsync("identity/register", new
            {
                email,
                password
            });

            if (response.IsSuccessStatusCode)
            {
                var loginResult = await _loginService.LoginAsync(email, password);
                if (loginResult.Success)
                {
                    return new RegisterResult(true);
                }
                else
                {
                    return new RegisterResult(true)
                    {
                        Errors = new[] { "Registration successful, but automatic login failed." }
                    };
                }
            }

            var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return new RegisterResult(false)
            {
                Errors = errorContent?.Errors ?? new[] { "Registration failed" }
            };
        }
        catch (Exception ex)
        {
            return new RegisterResult(false)
            {
                Errors = new[] { ex.Message }
            };
        }
    }
    private record ErrorResponse(string[] Errors);
}

public record RegisterResult(bool Success)
{
    public string[] Errors { get; set; } = Array.Empty<string>();
}