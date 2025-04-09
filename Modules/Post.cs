using System;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type {get;set;}
    public string Title { get; set; }
    public string Content { get; set; }
    public string MediaUrl { get; set; } 
    public decimal AmountGained { get; set; }
    public decimal TargetAmount {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Post(int userId, string title, string content, string mediaUrl, decimal amountGained, decimal targetAmount, string type)
    {

        this.UserId = userId;
        this.Title = title;
        this.Content = content;
        this.MediaUrl = mediaUrl;
        this.AmountGained = amountGained;
        this.TargetAmount = targetAmount;
        this.Type = type;
    }
}
