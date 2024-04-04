using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;
using SampleApp.RazorPage.Pages;


namespace SampleApp.Pages
{
    public class FollowedsModel : PageModel
    {
        private readonly HttpClient _http;
        private readonly ILogger<AuthModel> _log;
        private readonly IFlasher _f;
        public User ProfileUser { get; set; }

        public FollowedsModel(IHttpClientFactory factory, ILogger<AuthModel> log, IFlasher f)
        {
            _http = factory.CreateClient("API");
            _log = log;
            _f = f;
        }

        public IEnumerable<User> Followeds { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            //ProfileUser = _context.Users.Include(u => u.RelationFolloweds)
            //                            .ThenInclude(r => r.Follower)
            //                            .Where(u => u.Id == id)
            //                            .FirstOrDefault();

            //Followers = ProfileUser.RelationFolloweds.Select(item => item.Follower).ToList();

            //var sessionId = HttpContext.Session.GetString("SampleSession");

            var profileuser_resp = await _http.GetAsync($"{_http.BaseAddress}/users/{id}");

            if (!profileuser_resp.IsSuccessStatusCode)
            {
                return Page();
            }

            ProfileUser = await profileuser_resp.Content.ReadFromJsonAsync<User>();
            var followeds = await _http.GetAsync($"{_http.BaseAddress}/relations/Followeds/{id}");

            if (!followeds.IsSuccessStatusCode)
            {
                return Page();
            }
            Followeds = await followeds.Content.ReadFromJsonAsync<IEnumerable<User>>();

            return Page();
        }
    }
}
