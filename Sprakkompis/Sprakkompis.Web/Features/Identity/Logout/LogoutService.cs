namespace Sprakkompis.Web.Features.Identity.Logout;

public class LogoutService(HttpClient _httpClient)
{
    public async Task<bool> LogoutAsync()
    {
		try
		{
			var response = await _httpClient.PostAsync("api/identity/logout", null);
			return response.IsSuccessStatusCode;
		}
		catch (Exception)
		{
			return false;
        }
    }
}