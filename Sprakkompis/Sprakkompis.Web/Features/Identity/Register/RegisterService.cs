using System.ComponentModel;

namespace Sprakkompis.Web.Features.Identity.Register;

public class RegisterService(HttpClient _httpclient)
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
                return new RegisterResult { Success = true };
            }

            var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return new RegisterResult
            {
                Success = false,
                Errors = errorContent?.Errors ?? new[] { "Registration failed" }
            };
        }
        catch (Exception ex)
        {
            return new RegisterResult
            {
                Success = false,
                Errors = new[] { ex.Message }
            };
        }
    }
    private class ErrorResponse
    {
        public string[] Errors { get; set; }
    }
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
}