using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Application;
using SampleApp.Domen.Models;

namespace SampleApp.Pages
{
    public class EditModel : PageModel
    {
        private readonly SampleAppContext _context;
        private readonly IFlasher _f;
        private readonly ILogger<EditModel> _log;

        public EditModel(SampleAppContext context, IFlasher f, ILogger<EditModel> log)
        {
            _context = context;
            _f = f;
            _log = log;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            User = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (User == null)
            {
                return NotFound();
            }
            else
            {
                return Page();
            }
            
        }

        public async Task<IActionResult> OnPostAsync(User user)
        {

            _context.Attach(User).State = EntityState.Modified;


            if (!user.IsPasswordConfirmation())
            {
                _log.LogWarning($"Пароли должны совпадать!");
                _f.Flash(Types.Danger, "При редактировании пользователя возникла ошибка: пароли должны совпадать!", dismissable: true);
                return Page();
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
