using Core.Flash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;
using SampleApp.RazorPage.Pages;
using static System.Net.WebRequestMethods;

namespace SampleApp.Pages
{
    public class FollowersModel : PageModel
    {

        private readonly HttpClient _http;
        private readonly ILogger<AuthModel> _log;
        private readonly IFlasher _f;
        public User ProfileUser { get; set; }

        public FollowersModel(IHttpClientFactory factory, ILogger<AuthModel> log, IFlasher f)
        {
            _http = factory.CreateClient("API");
            _log = log;
            _f = f;
        }


        public IEnumerable<User> Followers { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            //ProfileUser = _context.Users.Include(u => u.RelationFolloweds)
            //                            .ThenInclude(r => r.Follower)
            //                            .Where(u => u.Id == id)
            //                            .FirstOrDefault();

            //Followers = ProfileUser.RelationFolloweds.Select(item => item.Follower).ToList();

            //var sessionId = HttpContext.Session.GetString("SampleSession");

            var profileuser_resp = await _http.GetAsync($"{_http.BaseAddress}/users/{id}");

            if(!profileuser_resp.IsSuccessStatusCode)
            {
                return Page();
            }

            ProfileUser = await profileuser_resp.Content.ReadFromJsonAsync<User>();

            var followers = await _http.GetAsync($"{_http.BaseAddress}/users/Followers/{id}");

            if(!followers.IsSuccessStatusCode)
            {
                return Page();
            }
            Followers = await followers.Content.ReadFromJsonAsync<IEnumerable<User>>();

            return Page();
        }
    }
}
