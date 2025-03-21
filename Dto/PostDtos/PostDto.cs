public class PostDto
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? MediaUrl { get; set; }
    public decimal AmountGained { get; set; }
    public decimal TargetAmount {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
