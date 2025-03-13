public class Donation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; }
    public User User{ get; set; }

    public Donation(){}
    public Donation(int userId, decimal amount, int postId )
    {
        UserId = userId;
        Amount = amount;
        PostId = postId;

    }
}
