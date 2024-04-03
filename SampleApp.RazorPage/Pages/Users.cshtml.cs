using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;

namespace SampleApp.Pages
{
    public class UsersModel : PageModel
    {

        private readonly SampleAppContext _context;
        private readonly IFlasher _f;
        private readonly ILogger<UsersModel> _log;
        private readonly HttpClient _http;

        public UsersModel(SampleAppContext context, IHttpClientFactory factory, IFlasher f, ILogger<UsersModel> log)
        {
            _http = factory.CreateClient("API");
            _context = context;
            _f = f;
            _log = log;
        }

        public IList<User> Users { get; set; }
        public User User { get; set; }
        public string sessionId { get; set; }
        public User current_user { get; set; }



        //public async Task<IActionResult> OnGetAsync()
        //{
        //    Users = await _context.Users.ToListAsync();
        //    return Page();
        //}

        //public async Task<IActionResult> OnGetRemoveAsync([FromQuery] int id)
        //{
        //    try
        //    {
        //        var user = await _context.Users.FindAsync(id);
        //        _context.Users.Remove(user);
        //        _context.SaveChanges();
        //        _f.Flash(Types.Success, $"Пользователь {user.Id} успешно удалён!", dismissable: true);
        //        return RedirectToPage();
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogError($"{ex.Message}");

        //    }
        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            sessionId = HttpContext.Session.GetString("SampleSession");
            var users_resp = await _http.GetAsync($"{_http.BaseAddress}/users");
            Users = await users_resp.Content.ReadFromJsonAsync<List<User>>();
            if (sessionId != null)
            {
                var current_user_resp = await _http.GetAsync($"{_http.BaseAddress}/users/{sessionId}");
                current_user = await current_user_resp.Content.ReadFromJsonAsync<User>();
            }

            return Page();
        }
        public async Task<IActionResult> OnGetRemoveAsync([FromQuery] int id)
        {
            
                var delete_resp = await _http.DeleteAsync($"{_http.BaseAddress}/users/{id}");

                if (!delete_resp.IsSuccessStatusCode)
                {
                    _log.LogError($"Ошибка удаления: {delete_resp.Content.ToString()}");
                    //_f.Flash(Types.Danger, $"Ошибка удаления пользователя : {delete_resp.Content.ToString()}");

                }
                else
                {
                    //_log.LogInformation($"Пользователь {deleted_user.Name} удалён!");
                    //_f.Flash(Types.Success, $"Пользователь {deleted_user.Name} удалён!");
                }
                return RedirectToPage();
            
        }

    }
}
