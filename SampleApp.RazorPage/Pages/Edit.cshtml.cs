using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SampleApp.Domen.Models;
using System.Runtime.InteropServices;
using System.Text;


namespace SampleApp.RazorPage.Pages
{
    public class EditModel : PageModel
    {
        
        private readonly HttpClient _http;
        private readonly ILogger<AuthModel> _log;
        private readonly IFlasher _f;

        public EditModel(IHttpClientFactory factory, ILogger<AuthModel> log, IFlasher f)
        {
            _http = factory.CreateClient("API");
            _log = log;
            _f = f;
        }

        public User User { get; set; }
        public User current_user { get; set; }
        public async Task OnGet()
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");

            User = await _http.GetFromJsonAsync<User>($"{_http.BaseAddress}/users/{sessionId}");

            var response = await _http.GetAsync($"{_http.BaseAddress}/users/{sessionId}");

            current_user = await response.Content.ReadFromJsonAsync<User>();
        }


        public async Task OnPost()
        {
            var content = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"{_http.BaseAddress}/users/{User.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                _log.LogError($"Пользователь обновлён!");
                _f.Success($"Пользователь обновлён!");
            }
            else
            {
                _log.LogError($"Ошибка при обновлении!");
                _f.Danger($"Ошибка при обновлении!");
            }
        }
    }
}