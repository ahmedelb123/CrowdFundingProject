using Microsoft.AspNetCore.SignalR;

public class Comment
{
    public int id { get; set; }
    public int userId {get; set;}
    public int postId {get; set;}
    public string text { get; set; }
    public DateTime CreatedAt { get; set; }

    public Comment(){}

    public Comment(int postId, int userId, string text){
      this.postId = postId;
      this.userId = userId;
      this.text = text;
    }

}
