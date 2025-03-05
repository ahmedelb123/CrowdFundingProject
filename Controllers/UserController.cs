using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly UserHandler _userService;

  public UserController(AppDbContext context, UserHandler userService)
  {
    _context = context;
    _userService = userService; // Inject UserHandler into the constructor
  }

  [HttpPost("createAccount")]
  public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto request)
  {
    try
    {
      var result = await _userService.CreateUser(request.Name, request.Surname, request.Email, request.Password);

      // If user creation fails (user already exists), return BadRequest (400)
      if (result.status == false)
      {
        return BadRequest(result); // Return 400 Bad Request with the status and message
      }

      // If user creation is successful, return OK (200)
      return Ok(result); // Return 200 OK with the success message
    }
    catch (Exception ex)
    {

      // Return a structured JSON error message
      return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
    }
  }

  [HttpPost("login")]

  public async Task<IActionResult> login([FromBody] LoginDto request)
  {
    var result = await _userService.Login(request.Email, request.Password);
    if (!result.status)
    {
      return BadRequest(result);
    }
    return Ok(result);
  }
}
