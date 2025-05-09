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
    _userService = userService; 
  }

  [HttpPost("createAccount")]
  public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto request)
  {
    try
    {

      // Proceed with user creation if the email is unique
      var result = await _userService.CreateUser(request.Name, request.Surname, request.Email, request.Password);

      // If user creation fails, return BadRequest with the status and message
      if (!result.Status)
      {
        return BadRequest(result); // Return 400 Bad Request with the status and message
      }

      return Ok(new
      {
        message = result.Message,
        status = result.Status,
      });
    }
    catch (Exception ex)
    {
      
      return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
    }
  }


  [HttpPost("login")]
  public async Task<IActionResult> login([FromBody] LoginDto request)
  {
    var result = await _userService.Login(request);

    if (!result.Status) 
    {
      return BadRequest(result);
    }

    

    return Ok(new
    {
      status = result.Status,
      message = result.Message,
      id = result.Id,
    });
  }
  [HttpGet("getUser/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var result = await _userService.getUserById(id);
            if (!result.Status)
                return NotFound(new { message = "Post not found!" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the post.", error = ex.Message });
        }
    }


  
  
}
