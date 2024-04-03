using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Domen.Models;

namespace SampleApp.RazorPage.Pages;

public class AuthModel : PageModel
{

    public User User { get; set; }

    private readonly HttpClient _http;
    private readonly ILogger<AuthModel> _log;
    private readonly IFlasher _f;
    public AuthModel(IHttpClientFactory factory, ILogger<AuthModel> log, IFlasher f)
    {
        _http = factory.CreateClient("API");
        _log = log;
        _f = f;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(User user)
    {
       // var response = await _http.PostAsJsonAsync<User>("https://localhost:7225/api/users/auth", user);

        var response = await _http.GetAsync($"{_http.BaseAddress}/users/auth?email={user.Email}&password={user.Password}");
       
        var current_user = await response.Content.ReadFromJsonAsync<User>();
        try
        {
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("SampleSession", $"{current_user.Id}");
                _f.Flash(Types.Success, $"����� ����������, {current_user.Name}!");
                return RedirectToPage("Index");
            }
            else
            {                
                _f.Flash(Types.Danger, $"�������� ����� ��� ������!");
                _log.LogError($"������ ���: {response.StatusCode}");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _log.LogError($"������: {ex.InnerException.Message}");
            _f.Flash(Types.Danger, $"������ �����������: {ex.InnerException.Message}", dismissable: false);
            return RedirectToPage("Auth");

        }
        
    }

    public IActionResult OnGetLogout()
    {
        // ����� ������
        HttpContext.Session.Clear();
        _f.Flash(Types.Success, $"������ �������!", dismissable: false);
        return RedirectToPage("Auth");
    }

}
