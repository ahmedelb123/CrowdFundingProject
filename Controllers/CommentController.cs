using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/comment")]
[ApiController]
public class CommentController : ControllerBase
{
  private readonly AppDbContext context;
  private readonly CommentHandler commentHandler;
  public CommentController(AppDbContext context, CommentHandler commentHandler)
  {
    this.context = context;
    this.commentHandler = commentHandler;
  }

  [HttpPost("addComment")]
  public async Task<IActionResult> addComment([FromBody] CommentDto commentDto)
  {
    try
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      commentDto.UserId = userId;
      var result = await commentHandler.addComment(commentDto.PostId, commentDto.UserId, commentDto.CommentText);
      
      if (!result.Status) 
      {
        return BadRequest(result);
      }
      // If comment creation is successful, return OK (200)
      return Ok(new
      {
        message = result.Message,
        status = result.Status
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
    }
  }

  [HttpGet("post/{postId}")]
  public async Task<IActionResult> getCommentOfPost(int postId)
  {
    try
    {
      var result = await commentHandler.GetAllCommentsOfPost(postId);

      return Ok(result);
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
    }

  }



}