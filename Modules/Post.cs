using System;

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign key reference to User
    public string Title { get; set; }
    public string Content { get; set; }
    public string Media_url {get; set; } // Stores file path of the image
    public int AmountGained {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Post(int userId, string title, string content, string Media_url, int AmountGained)
    {
        this.UserId = userId;
        this.Title = title;
        this.Content = content;
        this.Media_url = Media_url;
        this.AmountGained = AmountGained;
    }
}