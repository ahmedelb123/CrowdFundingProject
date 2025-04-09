public class CreatePostDto
{
    public int UserId { get; set; }
    public string Type {get ;set;}
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required decimal TargetAmount { get; set; }
    public string? MediaUrl { get; set; }
}
