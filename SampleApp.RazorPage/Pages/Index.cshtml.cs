using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Domen.Models;
using Microsoft.EntityFrameworkCore;

namespace SampleApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SampleAppContext _context;
        private readonly IFlasher _f;
        private readonly ILogger<EditModel> _log;

        public User CurrentUser { get; set; }
        public User ProfileUser { get; set; }
        public bool IsFollow { get; set; }
        public List<Micropost> Messages { get; set; } = new();
        public IEnumerable<User> Followeds { get; set; }
        public List<User> Users { get; set; } = new();

        public IndexModel(SampleAppContext context, IFlasher f, ILogger<EditModel> log)
        {
            _context = context;
            _f = f;
            _log = log;
        }



        public async Task<IActionResult> OnGetAsync([FromRoute] int? id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
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

            /*if (sessionId != null)
            {
                var User = await _context.Users.Include(u => u.Microposts)
                                      .Include(u => u.RelationFollowers).ThenInclude(r => r.Followed)
                                      .FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

                Followeds = User.RelationFollowers.Select(item => item.Followed).ToList();
                
                Users.AddRange(Followeds);
                Users.Add(User);

                    
            }*/
            //else
            //{
                Users = await _context.Users.ToListAsync();
            //}   

            foreach (var u in Users)
            {
                _context.Entry(u).Collection(u => u.Microposts).Load();
                Messages.AddRange(u.Microposts);
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string message)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            CurrentUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

            if (!string.IsNullOrWhiteSpace(message))
            {
                var m = new Micropost()
                {
                    Content = message,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = CurrentUser.Id,
                    //User = this.User
                };

                try
                {
                    _context.Microposts.Add(m);
                    _context.SaveChanges();
                    _f.Flash(Types.Success, $"Tweet!", dismissable: true);
                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    _log.Log(LogLevel.Error, $"Ошибка создания сообщения: {ex.InnerException.Message}");
                }


                return Page();
            }
            else
            {
                return Page();
            }

        }
        public async Task<IActionResult> OnGetDeleteAsync([FromQuery] int messageid)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            CurrentUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

            try
            {
                Micropost m = _context.Microposts.Find(messageid);
                _context.Microposts.Remove(m);
                _context.SaveChanges();
                _log.Log(LogLevel.Error, $"Удалено сообщение \"{m.Content}\" пользователя {CurrentUser.Name}!");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, $"Ошибка удаления сообщения: {ex.InnerException}");
                _log.Log(LogLevel.Error, $"Модель привязки из маршрута: {messageid}");
            }

            return Page();

        }
    }
}