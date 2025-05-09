using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/post")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostHandler _postService;

    public PostController(PostHandler postService)
    {
        _postService = postService;
    }

    // Create a Post

    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request)
    {
        try
        {
            var result = await _postService.CreatePost(request);

            if (!result.Status)
                return BadRequest(result);

            return Ok(new
            {
                message = result.Message,
                postId = result.PostId,
                postDetails = result.PostDetails
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the post.", error = ex.Message });
        }
    }

    // Get a Single Post by ID
    [HttpGet("getPost/{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        try
        {
            var result = await _postService.GetPostById(id);
            if (result == null)
                return NotFound(new { message = "Post not found!" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the post.", error = ex.Message });
        }
    }
    [HttpGet("getPostsByType/{type}")]
    public async Task<IActionResult> GetPostsByType(string type, int page, int pageSize)
    {
        try
        {
            var results = await _postService.GetPostsByType(type, page, pageSize);
            if (results == null)
                return NotFound(new { message = "No posts found for this type!" });

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving posts.", error = ex.Message });
        }
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllPosts(int page = 1, int pageSize = 10)
    {
        try
        {
            var result = await _postService.GetAllPosts(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching posts.", error = ex.Message });
        }
    }


    // Update a Post

    [HttpPut("updatePost/{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        try
        {

            var result = await _postService.UpdatePost(id, request);

            if (!result.Status)
                return BadRequest(new
                {
                    message = result.Message,
                    state = result.Status,
                });

            return Ok(new
            {
                message = result.Message,
                state = result.Status,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the post.", error = ex.Message });
        }
    }

    // Delete a Post
    [HttpDelete("DeletePost/{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {

            // Get the authenticated user's ID from the JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Get the 'IsAdmin' claim from the JWT token
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value ?? "false";
            bool isAdmin = bool.Parse(isAdminClaim);

            var result = await _postService.DeletePost(id, userId, isAdmin);

            if (!result.Status)
                return BadRequest(new
                {
                    message = result.Message,
                    state = result.Status,
                });

            return Ok(new
            {
                message = result.Message,
                state = result.Status,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the post.", error = ex.Message });
        }
    }
}
