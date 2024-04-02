using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;

namespace SampleApp.API.Controllers;

[Route("api/registration")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SampleAppContext _context;
    private readonly ILogger<RegistrationController> _log;

    public AuthController(SampleAppContext context, ILogger<RegistrationController> log)
    {
        _context = context;
        _log = log;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        return await _context.Users.ToListAsync();
    }

}
