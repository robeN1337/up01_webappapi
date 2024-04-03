using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;
using System;

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
    [HttpPost("reg")]
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


    // GET: api/Users/auth?email=a&password=b  // Авторизация по email и password
    [HttpGet("auth")]
    public async Task<ActionResult<User>> GetByEmailAndPassword(string email, string password)
    {

        User current_user = _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
        try
        {
            if (current_user != null)
            {
                HttpContext.Session.SetString("SampleSession", $"{current_user.Id}");
                return Ok();
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


    // DELETE: api/Users/5
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
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
