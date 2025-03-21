public class CommentDto
{
  public required int PostId { get; set; }
  public int UserId { get; set; }
  public required string CommentText { get; set; }
}