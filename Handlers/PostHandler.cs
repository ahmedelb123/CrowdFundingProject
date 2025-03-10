using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PostHandler
{
    private readonly AppDbContext _dbContext;

    public PostHandler(AppDbContext context)
    {
        _dbContext = context;
    }

    // Create a Post
    public async Task<ResponseDto> CreatePost(CreatePostDto postDto)
    {
        var newPost = new Post(postDto.UserId, postDto.Title, postDto.Content, postDto.MediaUrl ?? "", 0);
        
        _dbContext.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();

        return new ResponseDto { status = true, message = "Post created successfully!" };
    }

    // Get a Single Post by ID
    public async Task<PostDto?> GetPostById(int postId)
    {
        var post = await _dbContext.Posts.FindAsync(postId);
        if (post == null) return null;

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            MediaUrl = post.MediaUrl, 
            AmountGained = post.AmountGained,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    // Get All Posts
    public async Task<List<PostDto>> GetAllPosts()
    {
        var posts = await _dbContext.Posts.ToListAsync();
        return posts.ConvertAll(post => new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            AmountGained = post.AmountGained,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        });
    }

    // Update a Post
    public async Task<ResponseDto> UpdatePost(int postId, UpdatePostDto postDto)
    {
        var existingPost = await _dbContext.Posts.FindAsync(postId);
        if (existingPost == null)
        {
            return new ResponseDto { status = false, message = "Post not found!" };
        }

        existingPost.Title = postDto.Title;
        existingPost.Content = postDto.Content;
        existingPost.MediaUrl = postDto.MediaUrl ?? existingPost.MediaUrl;
        existingPost.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        
        return new ResponseDto { status = true, message = "Post updated successfully!" };
    }

    // Delete a Post
    public async Task<ResponseDto> DeletePost(int postId)
    {
        var post = await _dbContext.Posts.FindAsync(postId);
        if (post == null)
        {
            return new ResponseDto { status = false, message = "Post not found!" };
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();

        return new ResponseDto { status = true, message = "Post deleted successfully!" };
    }
}