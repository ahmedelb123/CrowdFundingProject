using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CommentHandler
{
  private const string USER_DONT_EXIST = "This user dont exist";
  private const string COMMENT_CREATED = "The Comment is created";
  private readonly AppDbContext _dbContext;

  public CommentHandler(AppDbContext context)
  {
    _dbContext = context;
  }

 public async Task<ResponseDto> addComment(int postId, int userId, string commentText)
{
    bool userExists = await _dbContext.Users.AnyAsync(u => u.id == userId);
    if (!userExists)
    {
        return new ResponseDto { Status = false, Message = USER_DONT_EXIST };
    }

    bool postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);
    if (!postExists)
    {
        return new ResponseDto { Status = false, Message = "Post does not exist" };
    }

    // Create Comment
    Comment newComment = new Comment(postId, userId, commentText);
    _dbContext.Comments.Add(newComment);
    await _dbContext.SaveChangesAsync();

    return new ResponseDto { Status = true, Message = COMMENT_CREATED };
}

  public async Task<List<Comment>> GetAllCommentsOfPost(int postId)
  {
    bool postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);
    if (!postExists)
    {
      throw new KeyNotFoundException($"Post with ID {postId} not found.");
    }

    return await _dbContext.Comments.Where(c => c.postId == postId).ToListAsync();
  }
}








