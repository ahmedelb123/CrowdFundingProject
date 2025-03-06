using System;

public class Post
{
    public int Id { get; set; }
    public int user_id { get; set; } // Foreign key reference to User
    public string title { get; set; }
    public string content { get; set; }
    public string media_url { get; set; } // Stores file path of the image
    public int amount_gained { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Post() { }
    public Post(int userId, string title, string content, string Media_url, int AmountGained)
    {
        this.user_id = userId;
        this.title = title;
        this.content = content;
        this.media_url = Media_url;
        this.amount_gained = AmountGained;
    }
}