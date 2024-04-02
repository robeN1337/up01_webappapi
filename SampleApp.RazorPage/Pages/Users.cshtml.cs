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

        public UsersModel(SampleAppContext context, IFlasher f, ILogger<UsersModel> log)
        {
            _context = context;
            _f = f;
            _log = log;
        }

        public IList<User> Users { get; set; }
        public new User User { get; set; }
        public string sessionId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetRemoveAsync([FromQuery] int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                _context.Users.Remove(user);
                _context.SaveChanges();
                _f.Flash(Types.Success, $"Пользователь {user.Id} успешно удалён!", dismissable: true);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _log.LogError($"{ex.Message}");
                
            }
            return Page();
        }

    }
}
