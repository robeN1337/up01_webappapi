using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;

namespace SampleApp.Pages
{
    public class ProfileModel : PageModel
    {

        private readonly SampleAppContext _context;
        private readonly IFlasher _f;
        private readonly ILogger<ProfileModel> _log;

        public User CurrentUser { get; set; }
        public User ProfileUser { get; set; }
        public bool IsFollow { get; set; }

        public ProfileModel(SampleAppContext context, IFlasher f, ILogger<ProfileModel> log)
        {
            _context = context;
            _f = f;
            _log = log;
        }

        public async Task<IActionResult> OnGetAsync([FromRoute]int? id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            ProfileUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == id) as User;   
            CurrentUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id.ToString() == sessionId) as User;

            // если текущий пользователь подписан на профиль пользователя
            var result = _context.Relations.Where(r => r.Follower == CurrentUser  && r.Followed == ProfileUser).FirstOrDefault();

            if (result != null) 
            {
              IsFollow = true;
            }
            else 
            {
              IsFollow =false;
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync([FromRoute] int? id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            ProfileUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == id) as User;
            CurrentUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id.ToString() == sessionId) as User;

            // если текущий пользователь подписан на профиль пользователя
            var result = _context.Relations.Where(r => r.Follower == CurrentUser && r.Followed == ProfileUser).FirstOrDefault();

            if (result != null)
            {
                IsFollow = true;
            }
            else
            {
                IsFollow = false;
            }

            if (IsFollow == false)
            {
                try
                {
                    _context.Relations.Add(new Relation() { FollowerId = CurrentUser.Id, FollowedId = ProfileUser.Id });
                    _context.SaveChanges();
                    _f.Flash(Types.Success, $"Пользователь {CurrentUser.Name} подписался на {ProfileUser.Name}!", dismissable: true);
                }
                catch (Exception ex)
                {
                    _f.Flash(Types.Success, $"{ex.InnerException.Message}", dismissable: true);
                }
            }
            else
            {

                try
                {
                    var result2 = _context.Relations.Where(r => r.Follower == CurrentUser && r.Followed == ProfileUser).FirstOrDefault();
                    _context.Relations.Remove(result2);
                    _context.SaveChanges();
                    _f.Flash(Types.Warning, $"Пользователь {CurrentUser.Name} отписался от {ProfileUser.Name}!", dismissable: true);
                }
                catch (Exception ex)
                {
                    _f.Flash(Types.Success, $"{ex.Message}", dismissable: true);
                }


            }

            return RedirectToPage();
        }
    }
}
