public class UpdatePostDto
{   
    public required int UserId {get; set;}
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? MediaUrl { get; set; }
}
