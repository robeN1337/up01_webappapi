using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Domen.Models;
using System.Security.AccessControl;

namespace SampleApp.RazorPage.Pages;

public class SignModel : PageModel
{
    private readonly HttpClient _http;
    private readonly ILogger<SignModel> _log;
    private readonly IFlasher _f;
    public SignModel(IHttpClientFactory factory, ILogger<SignModel> log, IFlasher f)
    {
        _http = factory.CreateClient("API");
        _log = log;
        _f = f;
    }

    public void OnGet()
    {
        Console.WriteLine("Загрузилась страница Sign");
    }

    public async Task<IActionResult> OnPostAsync(User user)
    {

        //if (!user.IsUniqEmail())
        //{
        //    ModelState.AddModelError("UniqEmail","Такой адре почты уже есть в базе данных!");
        //}

        //if (!ModelState.IsValid)
        //{
        //    _log.LogInformation($"Не корректные данные модели!");
        //    _f.Flash(Types.Danger, $"Не корректные данные модели!", dismissable: false);
        //    return Page();
        //}


        try
        {
            var response = await _http.PostAsJsonAsync<User>("https://localhost:7225/api/users", user);
            if (response.IsSuccessStatusCode)
            {
                _log.LogInformation($"Пользователь {user.Name} успешно зарегистрирован!");
                _f.Flash(Types.Success, $"Пользователь  {user.Name} зарегистрирован!", dismissable: false);
                return RedirectToPage("./Auth");
            }
            else
            {
                _log.LogError($"Статус код: {response.StatusCode}");
                return Page();
            }

        }
        catch (Exception ex)
        {
            _log.LogError($"Ошибка: {ex.InnerException.Message}");
            _f.Flash(Types.Danger, $"Ошибка регистрации: {ex.InnerException.Message}", dismissable: false);
            return RedirectToPage("./Sign");
        }
    }



}
