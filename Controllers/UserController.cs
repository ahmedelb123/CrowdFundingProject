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
        if (!result.Status)  // Use `Status` instead of `status`
        {
            return BadRequest(result); // Return 400 Bad Request with the status and message
        }

    return Ok(new 
        { 
            message = result.Message,
            status=result.Status,
        });
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

    if (!result.Status)  // Use `Status` instead of `status`
    {
        return BadRequest(result);
    }

    // Set the token in an HTTP-only cookie
    Response.Cookies.Append("authToken", result.Token, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddHours(2),
    });

    return Ok(new 
        { 
          status=result.Status,
          message = result.Message,
          id = result.Id,  
        });
}


  [HttpPost("logout")]
    public IActionResult Logout()
  {
    // Delete the authToken cookie
    Response.Cookies.Delete("authToken");
    
    return Ok(new { message = "Logged out successfully" });
  }
}
