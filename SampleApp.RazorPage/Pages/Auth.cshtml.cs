using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Domen.Models;

namespace SampleApp.Pages
{
    public class AuthModel : PageModel
    {
        private readonly SampleAppContext _db;
        private readonly ILogger<SampleAppContext> _logger;
        private readonly IFlasher _f;

        public AuthModel(SampleAppContext db, ILogger<SampleAppContext> logger, IFlasher f)
        {
            _db = db;
            _logger = logger;
            _f = f;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public User Input { get; set; }

        public IActionResult OnPost()
        {
            User current_user = _db.Users.Where(u => u.Email == Input.Email && u.Password == Input.Password).FirstOrDefault();
            if (current_user != null)
            {
                HttpContext.Session.SetString("SampleSession", $"{current_user.Id}");
                _f.Flash(Types.Success, $"Добро пожаловать, {current_user.Name}!", dismissable: true);
                return RedirectToPage("Index");
            }
            else
            {
                _f.Flash(Types.Danger, $"Неверный логин или пароль!", dismissable: true);
                return Page();
            }
        }
        
        public IActionResult OnGetLogout()
        {
            // сброс сессии
            
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("SampleSession");
            Response.Cookies.Delete("SampleSession");
            _f.Flash(Types.Success, $"Данные очищены!", dismissable: true);
            return RedirectToPage("Index");
        }
    }
}
