using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SampleApp.Domen.Application;
using SampleApp.Domen.Models;
using System.Text;

namespace SampleApp.API.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly SampleAppContext _context;
    private readonly ILogger<UsersController> _log;
        
    public UsersController(SampleAppContext context, ILogger<UsersController> log)
    {
        _context = context;
        _log = log;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        return await _context.Users.ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }



    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        if(!user.IsPasswordConfirmation())
        {
            return BadRequest("Пароли должны совпадать.");
        }    
        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Users // Регистрация пользователя
    [HttpPost]
    public async Task<ActionResult<User>> RegisterUser(User user)
    {
        _context.Users.Add(user);
        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("/Auth");
        }
        catch (Exception ex)
        {
            _log.LogError($"Не получилось зарегистрировать аккаунт: {ex.InnerException.Message}");
            return BadRequest(ex.InnerException.Message);
        }
    }
    


    
    [HttpGet("Followers/{id}")]
    public async Task<ActionResult<IEnumerable<User>>> GetFollowersById(int id)
    {
        if (_context.Relations == null)
        {
            return NotFound();
        }

        var relations = await _context.Relations.Where(r => r.FollowedId == id).ToListAsync();

        if (relations == null)
        {
            return NotFound();
        }
        User user = null;
        List<User> followers = new List<User>();

        foreach (var relation in relations)
        {
            user = await _context.Users.FindAsync(relation.FollowerId);
            user.RelationFolloweds = null;
            user.RelationFollowers = null;
            followers.Add(user);
        }
        //var content = new StringContent(JsonConvert.SerializeObject(followers), Encoding.UTF8, "application/json"); 
        return followers;
    }

    [HttpGet("Followeds/{id}")]
    public async Task<ActionResult<IEnumerable<User>>> GetFollowedsById(int id)
    {
        if (_context.Relations == null)
        {
            return NotFound();
        }
        var relations = await _context.Relations.Where(r => r.FollowerId == id).ToListAsync();

        if (relations == null)
        {
            return NotFound();
        }
        User user = null;
        List<User> followeds = new List<User>();

        foreach (var relation in relations)
        {
            user = await _context.Users.FindAsync(relation.FollowedId);
            user.RelationFolloweds = null;
            user.RelationFollowers = null;
            followeds.Add(user);
        }
        //var content = new StringContent(JsonConvert.SerializeObject(followers), Encoding.UTF8, "application/json"); 
        return followeds;
    }



    // GET: api/Users/auth?email=a&password=b  // Авторизация по email и password
    [HttpGet("auth")]
    public async Task<ActionResult<User>> GetByEmailAndPassword(string email, string password)
    {

        User current_user = _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
        try
        {
            if (current_user != null)
            {
                return Ok(current_user);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _log.LogError($"Ошибка авторизации: {ex.Message}");
            return BadRequest(ex.Message);
        }
        
    }



    // DELETE: api/Users/5 // Удаление пользователя админами через вкладку "Users"
    [HttpDelete("{id}")] 
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        catch (Exception ex)
        {
            _log.LogError($"Ошибка удаления пользователя: {ex.Message}");
            return BadRequest(ex.Message);
        }
        
        
    }

    
    
    
    
    
    
    
    
    private bool UserExists(int id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
