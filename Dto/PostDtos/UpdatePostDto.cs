public class UpdatePostDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? MediaUrl { get; set; }
}
