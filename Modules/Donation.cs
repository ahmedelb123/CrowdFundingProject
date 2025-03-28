public class Donation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int BankAccountId { get; set;}
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public Donation(){}
    public Donation(int userId, decimal amount, int postId, int bankAccountId )
    {
        UserId = userId;
        Amount = amount;
        PostId = postId;
        BankAccountId = bankAccountId;
        

    }
}
