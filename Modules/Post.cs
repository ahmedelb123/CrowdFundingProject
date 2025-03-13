using System;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign key reference to User
    public string Title { get; set; }
    public string Content { get; set; }
    public string MediaUrl { get; set; } // Stores file path of the image
    public int AmountGained { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Post(int userId, string title, string content, string mediaUrl, int amountGained)
    {
        this.UserId = userId;
        this.Title = title;
        this.Content = content;
        this.MediaUrl = mediaUrl;
        this.AmountGained = amountGained;
    }
}
