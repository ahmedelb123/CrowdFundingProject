public class CommentDto
{
  public required int PostId { get; set; }
  public required int UserId { get; set; }
  public required string CommentText { get; set; }
}