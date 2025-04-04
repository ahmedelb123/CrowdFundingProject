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
        try
        {

            var newPost = new Post(postDto.UserId, postDto.Title, postDto.Content, postDto.MediaUrl ?? "", 0, postDto.TargetAmount, postDto.Type);

            _dbContext.Posts.Add(newPost);
            await _dbContext.SaveChangesAsync();

            var postDetails = new PostDto
            {
                Id = newPost.Id,
                UserId = newPost.UserId,
                Title = newPost.Title,
                Type = newPost.Type,
                Content = newPost.Content,
                MediaUrl = newPost.MediaUrl,
                AmountGained = newPost.AmountGained,
                TargetAmount = newPost.TargetAmount,
                CreatedAt = newPost.CreatedAt,
                UpdatedAt = newPost.UpdatedAt
            };

            return new ResponseDto
            {
                Status = true,
                Message = "Post created successfully!",
                PostId = newPost.Id,
                PostDetails = postDetails
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto { Status = false, Message = "An error occurred while creating the post." };
        }
    }

    // Get a Single Post by ID
    public async Task<PostDto?> GetPostById(int postId)
    {
        try
        {
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null) return null;

            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                AmountGained = post.AmountGained,
                TargetAmount = post.TargetAmount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
    public async Task<PagedResult<PostDto>> GetPostsByType(string postType, int page, int pageSize)
    {
        try
        {
            // Get total count of posts matching the type
            var totalPosts = await _dbContext.Posts
                .Where(p => p.Type == postType)
                .CountAsync();

            // Get paginated posts
            var posts = await _dbContext.Posts
                .Where(p => p.Type == postType)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Type = post.Type,
                    Title = post.Title,
                    Content = post.Content,
                    MediaUrl = post.MediaUrl,
                    AmountGained = post.AmountGained,
                    TargetAmount = post.TargetAmount,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<PostDto>
            {
                TotalItems = totalPosts,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Items = posts
            };
        }
        catch (Exception)
        {
            return new PagedResult<PostDto>
            {
                TotalItems = 0,
                TotalPages = 0,
                CurrentPage = page,
                PageSize = pageSize,
                Items = new List<PostDto>()
            };
        }
    }



    public async Task<PagedResult<PostDto>> GetAllPosts(int page, int pageSize)
    {
        try
        {
            var totalPosts = await _dbContext.Posts.CountAsync();

            var posts = await _dbContext.Posts
                .OrderByDescending(p => p.CreatedAt) // Sort by newest posts first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = posts.ConvertAll(post => new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                MediaUrl = post.MediaUrl,
                AmountGained = post.AmountGained,
                TargetAmount = post.TargetAmount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            });

            return new PagedResult<PostDto>
            {
                TotalItems = totalPosts,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Items = postDtos
            };
        }
        catch (Exception)
        {
            return new PagedResult<PostDto>
            {
                TotalItems = 0,
                TotalPages = 0,
                CurrentPage = page,
                PageSize = pageSize,
                Items = new List<PostDto>()
            };
        }
    }


    // Update a Post
    public async Task<ResponseDto> UpdatePost(int postId, UpdatePostDto postDto)
    {
        try
        {
            var existingPost = await _dbContext.Posts.FindAsync(postId);
            if (existingPost == null)
            {
                return new ResponseDto { Status = false, Message = "Post not found!" };
            }

            // Check if the authenticated user is the owner of the post
            if (existingPost.UserId != postDto.UserId)
            {
                return new ResponseDto { Status = false, Message = "Unauthorized: You can only update your own posts." };
            }



            existingPost.Title = postDto.Title;
            existingPost.Content = postDto.Content;
            existingPost.MediaUrl = postDto.MediaUrl ?? existingPost.MediaUrl;
            existingPost.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new ResponseDto { Status = true, Message = "Post updated successfully!" };
        }
        catch (Exception ex)
        {
            return new ResponseDto { Status = false, Message = "An error occurred while updating the post." };
        }
    }

    // Delete a Post
    public async Task<ResponseDto> DeletePost(int postId, int userId, bool isAdmin)
    {
        try
        {
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null)
            {
                return new ResponseDto { Status = false, Message = "Post not found!" };
            }

            // Check if the user is the owner of the post or an admin
            if (post.UserId != userId && !isAdmin)
            {
                return new ResponseDto { Status = false, Message = "You can only delete your own posts or if you are an admin." };
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            return new ResponseDto { Status = true, Message = "Post deleted successfully!" };
        }
        catch (Exception ex)
        {
            return new ResponseDto { Status = false, Message = "An error occurred while deleting the post." };
        }
    }
}
