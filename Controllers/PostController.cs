using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        var result = await _postService.CreatePost(request);
        if (!result.status) return BadRequest(result);
        return Ok(result);
    }

    // Get a Single Post by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var result = await _postService.GetPostById(id);
        if (result == null) return NotFound(new { message = "Post not found!" });
        return Ok(result);
    }

    // Get All Posts
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPosts()
    {
        var result = await _postService.GetAllPosts();
        return Ok(result);
    }

    // Update a Post
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        var result = await _postService.UpdatePost(id, request);
        if (!result.status) return BadRequest(result);
        return Ok(result);
    }

    // Delete a Post
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var result = await _postService.DeletePost(id);
        if (!result.status) return BadRequest(result);
        return Ok(result);
    }
}