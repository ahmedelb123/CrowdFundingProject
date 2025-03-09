using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
      var result = await commentHandler.addComment(commentDto.PostId, commentDto.UserId, commentDto.CommentText);
      // If comment creation fails(user already exists), return BadRequest(400)
      if (result.status == false)
      {
        return BadRequest(result);
      }
      // If comment creation is successful, return OK (200)
      return Ok(result);
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